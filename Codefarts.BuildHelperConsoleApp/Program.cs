// <copyright file="Program.cs" company="Codefarts">
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

    partial class Program
    {
        static void Main(string[] args)
        {
            // Debugger.Launch();
            var build = new BuildHelper();
            build.OutputMessage += (s, e) => { Console.WriteLine(e.Message); };

            var buildFile = args.FirstOrDefault(x => x.StartsWith("-b:"));

            buildFile = string.IsNullOrWhiteSpace(buildFile) ? null : buildFile.Substring(3);

            // load command plugins
            var commands = LoadCommandPlugins(build).ToArray();
            build.Build(buildFile, commands);
        }

        private static IEnumerable<IBuildCommand> LoadCommandPlugins(BuildHelper buildHelper)
        {
            var appPath = Process.GetCurrentProcess().MainModule.FileName;
            var appDir = Path.GetDirectoryName(appPath);
            var pluginFolder = Path.Combine(appDir, "Plugins");

            if (!Directory.Exists(pluginFolder))
            {
                return Enumerable.Empty<IBuildCommand>();
            }

            var asmFiles = Directory.GetFiles(pluginFolder, "*.dll", SearchOption.AllDirectories);

            // load them
            var pluginTypes = asmFiles.SelectMany(f =>
            {
                var asm = Assembly.LoadFrom(f);
                return asm.GetTypes().Where(t => t.IsPublic && t.IsClass && !t.IsSealed && typeof(IBuildCommand).IsAssignableFrom(t));
            }).ToArray();

            var plugins = pluginTypes.Select(t =>
            {
                try
                {
                    return (IBuildCommand)t.Assembly.CreateInstance(t.FullName);
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

    // BuildHelper "-bf:$(ProjectDir)$(ConfigurationName)-PostBuild.xml" '-vs_OutDir:$(OutDir)' "-vs_ConfigurationName:$(ConfigurationName)" "-vs_ProjectName:$(ProjectName)" "-vs_TargetName:$(TargetName)" "-vs_TargetPath:$(TargetPath)" "-vs_ProjectPath:$(ProjectPath)" "-vs_ProjectFileName:$(ProjectFileName)" "-vs_TargetExt:$(TargetExt)" "-vs_TargetFileName:$(TargetFileName)" "-vs_DevEnvDir:$(DevEnvDir)" "-vs_TargetDir:$(TargetDir)" "-vs_ProjectDir:$(ProjectDir)" "-vs_SolutionFileName:$(SolutionFileName)" "-vs_SolutionPath:$(SolutionPath)" "-vs_SolutionDir:$(SolutionDir)" "-vs_SolutionName:$(SolutionName)" "-vs_PlatformName:$(PlatformName)" "-vs_ProjectExt:$(ProjectExt)" "-vs_SolutionExt:$(SolutionExt)"
}
