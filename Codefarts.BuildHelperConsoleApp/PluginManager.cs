// <copyright file="PluginManager.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace Codefarts.BuildHelperConsoleApp;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using BuildHelper;
using DependencyInjection;

public class PluginManager : IPluginManager
{
    private IStatusReporter status;
    private IDependencyInjectionProvider ioc;
    private readonly PluginCollection plugins;
    private string pluginFolder;
    private string pluginContextName;
    private bool useSeparatePluginContext = true;
    private string applicationPath;

    public PluginManager(IDependencyInjectionProvider ioc)
    {
        this.ioc = ioc ?? throw new ArgumentNullException(nameof(ioc));
        try
        {
            this.status = ioc.Resolve<IStatusReporter>();
        }
        catch
        {
        }

        IConfigurationProvider config = null;
        try
        {
            config = this.ioc.Resolve<IConfigurationProvider>();
            this.useSeparatePluginContext = config.GetValue<bool>("useseparateplugincontext", this.useSeparatePluginContext);
            this.pluginContextName = config.GetValue<string>("plugincontextname", this.pluginContextName);
        }
        catch
        {
        }

        this.GetPluginPath();
        this.plugins = this.Load();
    }

    private void GetPluginPath()
    {
        IConfigurationProvider config = null;
        try
        {
            config = this.ioc.Resolve<IConfigurationProvider>();
        }
        catch
        {
        }
        // if (config == null)
        // {
        //     throw new NullReferenceException($"A {nameof(IConfigurationProvider)} is not available.");
        // }

        this.applicationPath = Path.GetDirectoryName(config.GetValue("applicationpath", Assembly.GetEntryAssembly().Location));

        if (config != null && config.TryGetValue<string>("pluginfolder", out var folder))
        {
            if (string.IsNullOrEmpty(folder) || !Path.IsPathRooted(folder))
            {
                folder = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), folder);
                // throw new ArgumentException($"Plugin folder retrieved from the configuration provider but the path was not a full path.");
            }

            this.pluginFolder = folder;
        }
        else
        {
            this.pluginFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }
    }

    private PluginCollection LoadOld()
    {
        if (!Directory.Exists(this.pluginFolder))
        {
            return new PluginCollection();
        }

        var asmFiles = Directory.GetFiles(this.pluginFolder, "*.dll", SearchOption.AllDirectories);

        var pluginContext = this.useSeparatePluginContext
            ? AssemblyLoadContext.All.FirstOrDefault(x => x.Name.Equals(this.pluginContextName)) ??
              new AssemblyLoadContext(this.pluginContextName, true)
            : AssemblyLoadContext.Default;
        pluginContext.Resolving += this.ResolveAssemblies;

        /* 
               string inspectedAssembly = "Example.dll";
               var resolver = new PathAssemblyResolver(new string[] { inspectedAssembly, typeof(object).Assembly.Location });
               using var mlc = new MetadataLoadContext(resolver, typeof(object).Assembly.GetName().ToString());
       
               // Load assembly into MetadataLoadContext
               Assembly assembly = mlc.LoadFromAssemblyPath(inspectedAssembly);
               AssemblyName name = assembly.GetName();
       
               // Print types defined in assembly
               Console.WriteLine($"{name.Name} has following types: ");
       
               foreach (Type t in assembly.GetTypes())
               {
                   Console.WriteLine(t.FullName);
               }
       
                */
        // find types
        var pluginTypes = asmFiles
                          .Where(f => !pluginContext.Assemblies.Any(x => x.Location.Equals(f, StringComparison.InvariantCultureIgnoreCase)))
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

    private PluginCollection Load()
    {
        if (!Directory.Exists(this.pluginFolder))
        {
            this.status?.Report($"Plugin folder does not exist: '{this.pluginFolder}'.");
            return new PluginCollection();
        }

        // get list of all dll in plugin folder
        var asmFiles = Directory.GetFiles(this.pluginFolder, "*.dll", SearchOption.AllDirectories).ToList();
      //  asmFiles.AddRange(Directory.GetFiles(this.applicationPath, "*.exe", SearchOption.TopDirectoryOnly));
        asmFiles.AddRange(Directory.GetFiles(this.applicationPath, "*.dll", SearchOption.TopDirectoryOnly));

        // find plugin types without loading into a context
        var pluginTypes = this.FindPluginTypes(asmFiles);

        var pluginContext = this.useSeparatePluginContext ? new AssemblyLoadContext(this.pluginContextName, true) : AssemblyLoadContext.Default;
        pluginContext.Resolving += this.ResolveAssemblies;


        // only load assemblies with plugins into the plugin context
        foreach (var asmFile in pluginTypes.Select(x => x.File).Distinct())
        {
            pluginContext.LoadFromAssemblyPath(asmFile);
        }

        // resolve plugin tpes now that the plugin assemblies are loaded into the plugin context
        var plugins = pluginTypes.Select(t =>
        {
            try
            {
                Assembly? assembly = null;
                foreach (var x in pluginContext.Assemblies) //.Skip(70))
                {
                    if (x.Location.Equals(t.File, StringComparison.InvariantCultureIgnoreCase))
                    {
                        assembly = x;
                        break;
                    }
                }

                Type? type = null;
                foreach (var x in assembly.GetTypes())
                {
                    if (x.FullName == t.Type)
                    {
                        type = x;
                        break;
                    }
                }

                return this.ioc.Resolve(type) as ICommandPlugin;
            }
            catch (Exception ex)
            {
                this.status?.Report($"Failed to instantiate {t.Type} from assembly '{t.File}'.\r\n{ex}");
                //  AssemblyLoadContext.Default.Resolving -= this.ResolveAssemblies;
                return null;
            }
        }).Where(x => x != null).ToArray();

        this.status?.Report($"{plugins.Count()} plugins loaded.\r\n" + string.Join("\r\n", plugins.Select(x => x.Name)));

        pluginContext.Resolving -= this.ResolveAssemblies;
        return new PluginCollection(plugins);
    }

    private struct Entry
    {
        public string File;
        public string Type;
    }

    private IEnumerable<Entry> FindPluginTypes(IEnumerable<string> asmFiles)
    {
        Entry[] pluginTypes;
        try
        {
            // Get the array of runtime assemblies.
            // This will allow us to at least inspect types depending only on BCL.
            string[] runtimeAssemblies = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll");

            // Create the list of assembly paths consisting of runtime assemblies and the input file.
            var paths = new List<string>(runtimeAssemblies);
            paths.AddRange(asmFiles);

            // Create MetadataLoadContext that can resolve assemblies using the created list.
            var resolver = new PathAssemblyResolver(paths);
            var mlc = new MetadataLoadContext(resolver);

            //using (mlc)
            //{
            // find types
            pluginTypes = asmFiles.SelectMany(f =>
            {
                var asm = mlc.LoadFromAssemblyPath(f);
                // var items = new List<Type>();
                // foreach (var t in asm.GetTypes())
                // {
                //     if (t.IsPublic && t.IsClass && !t.IsAbstract && t.GetInterfaces().Any(x=>x.FullName== typeof(ICommandPlugin).FullName))// typeof(ICommandPlugin).IsAssignableFrom(t))
                //     {
                //         items.Add(t);
                //     }
                // }
                //
                // return items;


                return asm.GetTypes()
                          .Where(t => t.IsPublic && t.IsClass && !t.IsAbstract &&
                                      t.GetInterfaces().Any(x => x.FullName == typeof(ICommandPlugin).FullName))
                          .Select(x => new Entry() { File = x.Assembly.Location, Type = x.FullName });
                ;

                // return asm.GetTypes()
                //           .Where(t => t.IsPublic && t.IsClass && !t.IsAbstract && typeof(ICommandPlugin).IsAssignableFrom(t));
            }).ToArray();
            //}
        }
        catch
        {
            return Enumerable.Empty<Entry>();
        }

        return pluginTypes;
    }

    private Assembly? ResolveAssemblies(AssemblyLoadContext context, AssemblyName assemblyName)
    {
        if (!Directory.Exists(this.pluginFolder))
        {
            return null;
        }

        var fileMatches = Directory.GetFiles(this.pluginFolder, assemblyName.Name + ".dll", SearchOption.AllDirectories);
        var assemblyPath = fileMatches.FirstOrDefault();
        return !string.IsNullOrWhiteSpace(assemblyPath) && File.Exists(assemblyPath) ? context.LoadFromAssemblyPath(assemblyPath) : null;
    }

    public IEnumerable<ICommandPlugin> Plugins
    {
        get
        {
            return new List<ICommandPlugin>(this.plugins);
        }
    }
}