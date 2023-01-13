// <copyright file="PluginLoader.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelperConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;
    using BuildHelper;
    using DependencyInjection;

    internal class PluginLoader
    {
        private IStatusReporter status;
        private IDependencyInjectionProvider ioc;

        public PluginLoader(IDependencyInjectionProvider ioc)
        {
            this.ioc = ioc ?? throw new ArgumentNullException(nameof(ioc));
            this.status = ioc.Resolve<IStatusReporter>();
        }

        public string PluginFolder { get; set; }

        public string PluginContextName { get; set; }

        public bool UseSeparatePluginContext { get; set; }  

        public PluginCollection Load()
        {
            if (this.ioc == null)
            {
                throw new ArgumentNullException(nameof(this.ioc));
            }

            // if (UseSeparatePluginContext && string.IsNullOrWhiteSpace(this.PluginContextName))
            // {
            //     throw new ArgumentException($"If the {nameof(UseSeparatePluginContext)} property is true the {PluginContextName} property" +
            //                                 " can not be null or whitespace.");
            // }

            if (!Directory.Exists(this.PluginFolder))
            {
                return new PluginCollection();
            }

            var asmFiles = Directory.GetFiles(this.PluginFolder, "*.dll", SearchOption.AllDirectories);

            var pluginContext = this.UseSeparatePluginContext
                ? AssemblyLoadContext.All.FirstOrDefault(x => x.Name.Equals(this.PluginContextName)) ??
                  new AssemblyLoadContext(this.PluginContextName, true)
                : AssemblyLoadContext.Default;
            pluginContext.Resolving += this.ResolveAssemblies;

            // find types
            var pluginTypes = asmFiles
                              .Where(f => !pluginContext.Assemblies.Any(
                                         x => x.Location.Equals(f, StringComparison.InvariantCultureIgnoreCase)))
                              .SelectMany(f =>
                              {
                                  var asm = pluginContext.LoadFromAssemblyPath(f);
                                  return asm.GetTypes()
                                            .Where(t => t.IsPublic && t.IsClass && !t.IsAbstract && typeof(ICommandPlugin).IsAssignableFrom(t));
                              }).ToArray();

            // create them
            var plugins = pluginTypes.Select(t =>
            {
                try
                {
                    return this.ioc.Resolve(t) as ICommandPlugin;
                }
                catch
                {
                    this.status?.Report($"Failed to instantiate {t.FullName} from assembly '{t.Assembly.Location}'.");
                    AssemblyLoadContext.Default.Resolving -= this.ResolveAssemblies;
                    return null;
                }
            }).Where(x => x != null);

            this.status?.Report($"{plugins.Count()} plugins loaded.\r\n" + string.Join("\r\n", plugins.Select(x => x.Name)));

            pluginContext.Resolving -= this.ResolveAssemblies;
            return new PluginCollection(plugins);
        }

        private Assembly? ResolveAssemblies(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            var folderPaths = new List<string>();
            folderPaths.Add(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            folderPaths.Add(this.PluginFolder);
            var filter = assemblyName;

            switch (Path.GetExtension(filter.Name.ToLowerInvariant()))
            {
                case ".resources":
                    return null;
            }

            foreach (var folderPath in folderPaths)
            {
                if (!Directory.Exists(folderPath))
                {
                    continue;
                }

                var fileMatches = Directory.GetFiles(folderPath, filter.Name + ".dll", SearchOption.AllDirectories);
                var assemblyPath = fileMatches.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(assemblyPath) && File.Exists(assemblyPath))
                {
                    return context.LoadFromAssemblyPath(assemblyPath);
                }
            }

            return null;
        }
    }
}