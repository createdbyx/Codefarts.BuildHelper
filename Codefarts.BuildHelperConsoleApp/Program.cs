﻿// <copyright file="Program.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System.Collections.Generic;
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
        var buildFile = args.FirstOrDefault(x => x.StartsWith("-b:"));
        buildFile = string.IsNullOrWhiteSpace(buildFile) ? null : buildFile.Substring(3);
        var values = new Dictionary<string, object>();
        values["filename"] = buildFile;

        var ioc = new DependencyInjectorShim(new Container());
        ioc.Register<IDependencyInjectionProvider>(() => ioc);
        ioc.Register<IConfigurationManager>(() => new ConfigManager(values));
        ioc.Register<IStatusReporter, ConsoleStatusReporter>();
        ioc.Register<ICommandImporter>(() => new XmlCommandFileReader(ioc));

        var app = ioc.Resolve<Application>();
        app.Run();
    }
}

// BuildHelper "-bf:$(ProjectDir)$(ConfigurationName)-PostBuild.xml" '-vs_OutDir:$(OutDir)' "-vs_ConfigurationName:$(ConfigurationName)" "-vs_ProjectName:$(ProjectName)" "-vs_TargetName:$(TargetName)" "-vs_TargetPath:$(TargetPath)" "-vs_ProjectPath:$(ProjectPath)" "-vs_ProjectFileName:$(ProjectFileName)" "-vs_TargetExt:$(TargetExt)" "-vs_TargetFileName:$(TargetFileName)" "-vs_DevEnvDir:$(DevEnvDir)" "-vs_TargetDir:$(TargetDir)" "-vs_ProjectDir:$(ProjectDir)" "-vs_SolutionFileName:$(SolutionFileName)" "-vs_SolutionPath:$(SolutionPath)" "-vs_SolutionDir:$(SolutionDir)" "-vs_SolutionName:$(SolutionName)" "-vs_PlatformName:$(PlatformName)" "-vs_ProjectExt:$(ProjectExt)" "-vs_SolutionExt:$(SolutionExt)"