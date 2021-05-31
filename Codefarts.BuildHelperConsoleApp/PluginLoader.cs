// <copyright file="PluginLoader.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelperConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Codefarts.BuildHelper;

    internal class PluginLoader
    {
        public static IEnumerable<ICommandPlugin> LoadCommandPlugins(BuildHelper buildHelper)
        {
            if (buildHelper == null)
            {
                throw new ArgumentNullException(nameof(buildHelper));
            }

            var appPath = Process.GetCurrentProcess().MainModule.FileName;
            var appDir = Path.GetDirectoryName(appPath);
            var pluginFolder = Path.Combine(appDir, "Plugins");

            if (!Directory.Exists(pluginFolder))
            {
                return Enumerable.Empty<ICommandPlugin>();
            }

            var asmFiles = Directory.GetFiles(pluginFolder, "*.dll", SearchOption.AllDirectories);

            // load them
            var pluginTypes = asmFiles.SelectMany(f =>
            {
                var asm = Assembly.LoadFrom(f);
                return asm.GetTypes().Where(t => t.IsPublic && t.IsClass && !t.IsSealed && typeof(ICommandPlugin).IsAssignableFrom(t));
            }).ToArray();

            var plugins = pluginTypes.Select(t =>
            {
                try
                {
                    return (ICommandPlugin)t.Assembly.CreateInstance(t.FullName);
                }
                catch
                {
                    buildHelper.Output($"Failed to instantiate {t.FullName} from assembly '{t.Assembly.Location}'.");
                    return null;
                }
            }).Where(x => x != null);

            buildHelper.Output($"{plugins.Count()} plugins loaded.");
            buildHelper.Output(string.Join("\r\n", plugins.Select(x => x.Name)));

            return plugins;
        }
    }
}