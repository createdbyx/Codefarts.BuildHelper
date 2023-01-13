// <copyright file="Program.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System.Collections.Generic;

namespace Codefarts.BuildHelperConsoleApp;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Codefarts.BuildHelper;
using Codefarts.DependencyInjection;
using Codefarts.IoC;
using Codefats.BuildHelper.ConsoleReporter;

static class Program
{
    private static void Main(string[] args)
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

        IEnumerable<CommandData> buildFileCommands;
        var buildFileReader = ioc.Resolve<XmlBuildFileReader>();
        if (!buildFileReader.TryReadBuildFile(buildFile, out buildFileCommands))
        {
            status?.Report($"ERROR: Reading Build File. {buildFile}");
            Environment.ExitCode = 1;
            return;
        }

        var buildEventValue = variables.GetValue<string>("BuildEvent", null);
        status?.ReportHeader($"START {buildEventValue} BUILD");
        buildFileCommands.Run(variables, commandPlugins, status);
        status?.ReportHeader($"END {buildEventValue} BUILD");
    }

    private static void ReportHeader(this IStatusReporter status, string message, params object[] args)
    {
        var formattedString = string.Format(message, args);
        var maxLen = Math.Max(formattedString.Length + 10, 100);
        var headPartLen = (maxLen - (formattedString.Length + 2)) / 2;
        var headerChars = new string('#', headPartLen);
        status.Report(string.Format($"{headerChars} {message} {headerChars}", args));
    }
}

// BuildHelper "-bf:$(ProjectDir)$(ConfigurationName)-PostBuild.xml" '-vs_OutDir:$(OutDir)' "-vs_ConfigurationName:$(ConfigurationName)" "-vs_ProjectName:$(ProjectName)" "-vs_TargetName:$(TargetName)" "-vs_TargetPath:$(TargetPath)" "-vs_ProjectPath:$(ProjectPath)" "-vs_ProjectFileName:$(ProjectFileName)" "-vs_TargetExt:$(TargetExt)" "-vs_TargetFileName:$(TargetFileName)" "-vs_DevEnvDir:$(DevEnvDir)" "-vs_TargetDir:$(TargetDir)" "-vs_ProjectDir:$(ProjectDir)" "-vs_SolutionFileName:$(SolutionFileName)" "-vs_SolutionPath:$(SolutionPath)" "-vs_SolutionDir:$(SolutionDir)" "-vs_SolutionName:$(SolutionName)" "-vs_PlatformName:$(PlatformName)" "-vs_ProjectExt:$(ProjectExt)" "-vs_SolutionExt:$(SolutionExt)"