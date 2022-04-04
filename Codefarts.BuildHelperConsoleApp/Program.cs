// <copyright file="Program.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelperConsoleApp
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Codefarts.BuildHelper;
    using Codefarts.DependencyInjection;
    using Codefarts.IoC;
    using Codefats.BuildHelper.ConsoleReporter;

    static class Program
    {
        static void Main(string[] args)
        {
            var ioc = new DependencyInjectorShim(new Container());
            ioc.Register<IDependencyInjectionProvider>(() => ioc);
            ioc.Register<IStatusReporter, ConsoleStatusReporter>();

            var status = ioc.Resolve<IStatusReporter>();

            var buildFile = args.FirstOrDefault(x => x.StartsWith("-b:"));
            buildFile = string.IsNullOrWhiteSpace(buildFile) ? null : buildFile.Substring(3);

            // load command plugins
            var pluginLoader = ioc.Resolve<PluginLoader>();
            var appPath = Process.GetCurrentProcess().MainModule.FileName;
            var appDir = Path.GetDirectoryName(appPath);
            var pluginFolder = Path.Combine(appDir, "Plugins");

            pluginLoader.PluginFolder = pluginFolder;
            var commandPlugins = pluginLoader.Load();

            // read build file
            var variables = new VariablesDictionary();
            variables["Application"] = Path.GetFileName(appPath);

            XElement root;
            var buildFileReader = ioc.Resolve<BuildFileReader>();
            if (!buildFileReader.TryReadBuildFile(buildFile, out root))
            {
                status.Report($"ERROR: Reading Build File. {buildFile}");
                Environment.ExitCode = 1;
                return;
            }

            var buildFileCommands = root.Elements().Select(x => BuildCommandNode(x, null));

            var buildEventValue = variables.GetValue<string>("BuildEvent", null);
            status.ReportHeader($"START {buildEventValue} BUILD");
            buildFileCommands.Run(variables, commandPlugins, status);
            status.ReportHeader($"END {buildEventValue} BUILD");
        }

        public static void ReportHeader(this IStatusReporter status, string message, params object[] args)
        {
            var formattedString = string.Format(message, args);
            var maxLen = Math.Max(formattedString.Length + 10, 100);
            var headPartLen = (maxLen - (formattedString.Length + 2)) / 2;
            var headerChars = new string('#', headPartLen);
            status.Report(string.Format($"{headerChars} {message} {headerChars}", args));
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