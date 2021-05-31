// <copyright file="Program.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelperConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Codefarts.BuildHelper;

    class Program
    {
        static void Main(string[] args)
        {
            // Debugger.Launch();
            var build = new BuildHelper();
            build.OutputMessage += (s, e) =>
            {
                var categoryText = string.IsNullOrWhiteSpace(e.Category) ? string.Empty : $"(Category: {e.Category}) ";
                var typeText = string.IsNullOrWhiteSpace(e.Type) ? string.Empty : $"(Type: {e.Type}) ";
                Console.WriteLine($"{categoryText}{typeText}{e.Message}");
            };

            var buildFile = args.FirstOrDefault(x => x.StartsWith("-b:"));

            buildFile = string.IsNullOrWhiteSpace(buildFile) ? null : buildFile.Substring(3);

            // load command plugins
            var commands = PluginLoader.LoadCommandPlugins(build).ToArray();

            // read build file
            IDictionary<string, object> variables;
            XElement root;
            var buildFileReader = new BuildFileReader(build);
            if (!buildFileReader.TryReadBuildFile(buildFile, out variables, out root))
            {
                build.Output($"ERROR: Reading Build File. {buildFile}");
                Environment.ExitCode = 1;
                return;
            }

            var buildFileCommands = root.Elements().Select(x => BuildCommandNode(x, null));

            build.Run(buildFileCommands, variables, commands);
        }

        private static CommandData BuildCommandNode(XElement xElement, CommandData parent)
        {
            var node = new CommandData(xElement.Name.LocalName);
            foreach (var attribute in xElement.Attributes())
            {
                node.Parameters[attribute.Name.LocalName] = attribute.Value;
            }

            if (!string.IsNullOrWhiteSpace(xElement.Value) && !node.Parameters.ContainsKey("value"))
            {
                node.Parameters["value"] = xElement.Value;
            }

            node.Parent = parent;
            foreach (var childItem in xElement.Elements())
            {
                var newData = BuildCommandNode(childItem, node);
                node.Children.Add(newData);
            }

            return node;
        }
    }

    // BuildHelper "-bf:$(ProjectDir)$(ConfigurationName)-PostBuild.xml" '-vs_OutDir:$(OutDir)' "-vs_ConfigurationName:$(ConfigurationName)" "-vs_ProjectName:$(ProjectName)" "-vs_TargetName:$(TargetName)" "-vs_TargetPath:$(TargetPath)" "-vs_ProjectPath:$(ProjectPath)" "-vs_ProjectFileName:$(ProjectFileName)" "-vs_TargetExt:$(TargetExt)" "-vs_TargetFileName:$(TargetFileName)" "-vs_DevEnvDir:$(DevEnvDir)" "-vs_TargetDir:$(TargetDir)" "-vs_ProjectDir:$(ProjectDir)" "-vs_SolutionFileName:$(SolutionFileName)" "-vs_SolutionPath:$(SolutionPath)" "-vs_SolutionDir:$(SolutionDir)" "-vs_SolutionName:$(SolutionName)" "-vs_PlatformName:$(PlatformName)" "-vs_ProjectExt:$(ProjectExt)" "-vs_SolutionExt:$(SolutionExt)"
}