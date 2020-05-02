// <copyright file="Program.cs" company="Codefarts">
// Copyright (c) Codefarts
// </copyright>

using System.Diagnostics;

namespace Codefarts.BuildHelperConsoleApp
{
    using System;
    using System.Linq;
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
            //if (buildFile == null)
            //{
            //    build.Output("Build");
            //}

            build.Build(buildFile);
        }
    }

    // BuildHelper "-bf:$(ProjectDir)$(ConfigurationName)-PostBuild.xml" '-vs_OutDir:$(OutDir)' "-vs_ConfigurationName:$(ConfigurationName)" "-vs_ProjectName:$(ProjectName)" "-vs_TargetName:$(TargetName)" "-vs_TargetPath:$(TargetPath)" "-vs_ProjectPath:$(ProjectPath)" "-vs_ProjectFileName:$(ProjectFileName)" "-vs_TargetExt:$(TargetExt)" "-vs_TargetFileName:$(TargetFileName)" "-vs_DevEnvDir:$(DevEnvDir)" "-vs_TargetDir:$(TargetDir)" "-vs_ProjectDir:$(ProjectDir)" "-vs_SolutionFileName:$(SolutionFileName)" "-vs_SolutionPath:$(SolutionPath)" "-vs_SolutionDir:$(SolutionDir)" "-vs_SolutionName:$(SolutionName)" "-vs_PlatformName:$(PlatformName)" "-vs_ProjectExt:$(ProjectExt)" "-vs_SolutionExt:$(SolutionExt)"
}
