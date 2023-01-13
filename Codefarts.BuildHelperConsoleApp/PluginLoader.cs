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

        public PluginCollection Load()
        {
            if (this.ioc == null)
            {
                throw new ArgumentNullException(nameof(this.ioc));
            }

            if (!Directory.Exists(this.PluginFolder))
            {
                return new PluginCollection();
            }

            var asmFiles = Directory.GetFiles(this.PluginFolder, "*.dll", SearchOption.AllDirectories);

            AssemblyLoadContext.Default.Resolving += this.ResolveAssemblies;

            // find types
            var pluginTypes = asmFiles
                              .Where(f => !AssemblyLoadContext.Default.Assemblies.Any(
                                         x => x.Location.Equals(f, StringComparison.InvariantCultureIgnoreCase)))
                              .SelectMany(f =>
                              {
                                  var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(f);
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

            AssemblyLoadContext.Default.Resolving -= this.ResolveAssemblies;
            return new PluginCollection(plugins);
        }

        private Assembly? ResolveAssemblies(AssemblyLoadContext arg1, AssemblyName arg2)
        {
            var folderPaths = new List<string>();
            folderPaths.Add(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            folderPaths.Add(this.PluginFolder);
            var filter = arg2;

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
                    return arg1.LoadFromAssemblyPath(assemblyPath);
                }
            }

            return null;
        }
    }
}