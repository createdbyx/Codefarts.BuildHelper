// <copyright file="Program.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Codefarts.BuildHelper;
using Codefarts.DependencyInjection;
using Codefarts.IoC;
using Codefats.BuildHelper.ConsoleReporter;

namespace Codefarts.BuildHelperConsoleApp;

using System.Linq;

static class Program
{
    private static void Main(string[] args)
    {
        var buildFile = args.FirstOrDefault(x => x.StartsWith("-b:", StringComparison.InvariantCultureIgnoreCase));
        var projectFile = args.FirstOrDefault(x => x.StartsWith("-p:", StringComparison.InvariantCultureIgnoreCase));
        var silentMode = args.Any(x => x.Equals("-s", StringComparison.InvariantCultureIgnoreCase));
        var targetFramework = args.FirstOrDefault(x => x.StartsWith("-tf:", StringComparison.InvariantCultureIgnoreCase));
            
        // validate files exist
        if (IsFileMissing(buildFile, true, silentMode))
        {
            return;
        }

        if (IsFileMissing(projectFile, false, silentMode))
        {
            return;
        }

        // get default plugin folder
        var appPath = Process.GetCurrentProcess().MainModule.FileName;
        var appDir = Path.GetDirectoryName(appPath);
        var pluginFolder = Path.Combine(appDir, "Plugins");

        // store values
        var values = new Dictionary<string, object>();
        values["filename"] = buildFile;
        values["projectfile"] = projectFile;
        values["targetframework"] = targetFramework;
        values["applicationpath"] = appPath;
        values["pluginfolder"] = pluginFolder;
        var configurationManager = new ConfigManager(values);

        // do initialization
        var ioc = new DependencyInjectorShim(new Container());
        ioc.Register<IDependencyInjectionProvider>(() => ioc);
        ioc.Register<IConfigurationManager>(() => configurationManager);
        ioc.Register<IStatusReporter, ConsoleStatusReporter>();
        ioc.Register<ICommandImporter>(() => new XmlCommandFileReader(ioc));

        // create app and run
        var app = ioc.Resolve<Application>();
        var result = app.Run();
        if (result.Error != null)
        {
            Environment.ExitCode = 2;
        }
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