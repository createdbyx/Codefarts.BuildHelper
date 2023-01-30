// <copyright file="Program.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Codefarts.BuildHelper;
using Codefarts.DependencyInjection;
using Codefarts.IoC;
using Codefarts.XMLFileConfigManager;
using Codefats.BuildHelper.ConsoleReporter;

namespace Codefarts.BuildHelperConsoleApp;

using System.Linq;

static class Program
{
    private static void Main(string[] args)
    {
        var appPath = Process.GetCurrentProcess().MainModule.FileName;
        var appDir = Path.GetDirectoryName(appPath);


        var buildFile = GetArgument(args, "-b:");
        var projectFile = GetArgument(args, "-p:");
        var silentMode = args.Any(x => x.Equals("-s", StringComparison.InvariantCultureIgnoreCase));
        var targetFramework = GetArgument(args, "-tf:");
        var configFile = GetArgument(args, "-cf:") ?? Path.Combine(appDir, "config.xml");

        // validate files exist
        if (IsFileMissing(buildFile, true, silentMode) || IsFileMissing(projectFile, false, silentMode))
        {
            return;
        }

        // do initialization
        var ioc = new DependencyInjectorShim(new Container());
        ioc.Register<IDependencyInjectionProvider>(() => ioc);
        var xmlConfigProvider = ioc.Resolve<XmlFileConfigProvider>();

        xmlConfigProvider.Load(configFile);
        // store values
        xmlConfigProvider.SetValue("filename", buildFile);
        xmlConfigProvider.SetValue("projectfile", projectFile);
        xmlConfigProvider.SetValue("targetframework", targetFramework);
        xmlConfigProvider.SetValue("applicationpath", appPath);
        xmlConfigProvider.SetValue("configfile", configFile);
        
        ioc.Register<IConfigurationProvider>(() => xmlConfigProvider);
        ioc.Register<IStatusReporter, ConsoleStatusReporter>();
        ioc.Register<ICommandImporter>(() => new XmlCommandFileReader(ioc));
        var plugManager = ioc.Resolve<PluginManager>();
        ioc.Register<IPluginManager>(() => plugManager);

        // create app and run
        var app = ioc.Resolve<Application>();
        var result = app.Run();
        if (result.Error != null)
        {
            Environment.ExitCode = 2;
            if (!silentMode)
            {
                Console.Write(result.Error.ToString());
                Console.WriteLine();
            }
        }
    }

    private static string? GetArgument(string[] args, string header)
    {
        return args.Where(x => x.StartsWith(header, StringComparison.InvariantCultureIgnoreCase))
                   .Select(x => x.Substring(header.Length)).FirstOrDefault();
    }

    private static bool IsFileMissing(string filename, bool buildFile, bool silentMode)
    {
        // if file exists we are good to exit
        var buildFileInfo = new FileInfo(filename);
        if (buildFileInfo.Exists)
        {
            return false;
        }

        // file does not seem to exist
        Environment.ExitCode = 1;
        if (!silentMode)
        {
            var text = buildFile ? "Build" : "Project";
            Console.WriteLine($"{text} file not found!");
            Console.WriteLine($"File: {filename}");
        }

        return true;
    }
}

// BuildHelper "-bf:$(ProjectDir)$(ConfigurationName)-PostBuild.xml" '-vs_OutDir:$(OutDir)' "-vs_ConfigurationName:$(ConfigurationName)" "-vs_ProjectName:$(ProjectName)" "-vs_TargetName:$(TargetName)" "-vs_TargetPath:$(TargetPath)" "-vs_ProjectPath:$(ProjectPath)" "-vs_ProjectFileName:$(ProjectFileName)" "-vs_TargetExt:$(TargetExt)" "-vs_TargetFileName:$(TargetFileName)" "-vs_DevEnvDir:$(DevEnvDir)" "-vs_TargetDir:$(TargetDir)" "-vs_ProjectDir:$(ProjectDir)" "-vs_SolutionFileName:$(SolutionFileName)" "-vs_SolutionPath:$(SolutionPath)" "-vs_SolutionDir:$(SolutionDir)" "-vs_SolutionName:$(SolutionName)" "-vs_PlatformName:$(PlatformName)" "-vs_ProjectExt:$(ProjectExt)" "-vs_SolutionExt:$(SolutionExt)"