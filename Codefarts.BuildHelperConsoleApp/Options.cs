// <copyright file="Program.cs" company="Codefarts">
// Copyright (c) Codefarts
// </copyright>

using CommandLine;

namespace Codefarts.BuildHelperConsoleApp
{
    partial class Program
    {
        public class Options
        {
            [Option('o', nameof(vs_OutDir), Required = false, HelpText = "Specifies project out directory.")]
            public string vs_OutDir
            {
                get; set;
            }

            [Option('c', nameof(vs_ConfigurationName), Required = false, HelpText = "Specifies the configuration name.")]
            public string vs_ConfigurationName
            {
                get; set;
            }

            [Option('p', nameof(vs_ProjectName), Required = false, HelpText = "The project name.")]
            public string vs_ProjectName
            {
                get; set;
            }

            [Option('t', nameof(vs_TargetName), Required = false, HelpText = "The Target name.")]
            public string vs_TargetName
            {
                get; set;
            }

            [Option('r', nameof(vs_TargetPath), Required = false, HelpText = "Specifies the target path.")]
            public string vs_TargetPath
            {
                get; set;
            }

            [Option('h', nameof(vs_ProjectPath), Required = false, HelpText = "Set output to verbose messages.")]
            public string vs_ProjectPath
            {
                get; set;
            }

            [Option('f', nameof(vs_ProjectFileName), Required = false, HelpText = "Set output to verbose messages.")]
            public string vs_ProjectFileName
            {
                get; set;
            }

            [Option('e', nameof(vs_TargetExt), Required = false, HelpText = "Set output to verbose messages.")]
            public string vs_TargetExt
            {
                get; set;
            }

            [Option('n', nameof(vs_TargetFileName), Required = false, HelpText = "Set output to verbose messages.")]
            public string vs_TargetFileName
            {
                get; set;
            }

            [Option('d', nameof(vs_DevEnvDir), Required = false, HelpText = "Set output to verbose messages.")]
            public string vs_DevEnvDir
            {
                get; set;
            }

            [Option('i', nameof(vs_TargetDir), Required = false, HelpText = "Set output to verbose messages.")]
            public string vs_TargetDir
            {
                get; set;
            }

            [Option('j', nameof(vs_ProjectDir), Required = false, HelpText = "Set output to verbose messages.")]
            public string vs_ProjectDir
            {
                get; set;
            }

            [Option('s', nameof(vs_SolutionFileName), Required = false, HelpText = "Set output to verbose messages.")]
            public string vs_SolutionFileName
            {
                get; set;
            }

            [Option('u', nameof(vs_SolutionPath), Required = false, HelpText = "Set output to verbose messages.")]
            public string vs_SolutionPath
            {
                get; set;
            }

            [Option('l', nameof(vs_SolutionDir), Required = false, HelpText = "Set output to verbose messages.")]
            public string vs_SolutionDir
            {
                get; set;
            }

            [Option('m', nameof(vs_SolutionName), Required = false, HelpText = "Set output to verbose messages.")]
            public string vs_SolutionName
            {
                get; set;
            }

            [Option('a', nameof(vs_PlatformName), Required = false, HelpText = "Set output to verbose messages.")]
            public string vs_PlatformName
            {
                get; set;
            }

            [Option('x', nameof(vs_ProjectExt), Required = false, HelpText = "Set output to verbose messages.")]
            public string vs_ProjectExt
            {
                get; set;
            }

            [Option('z', nameof(vs_SolutionExt), Required = false, HelpText = "Set output to verbose messages.")]
            public string vs_SolutionExt
            {
                get; set;
            }

            [Option('b', nameof(BuildFile), Required = true, HelpText = "Specifies the build file.")]
            public string BuildFile
            {
                get; set;
            }
        }
    }

    // BuildHelper "-bf:$(ProjectDir)$(ConfigurationName)-PostBuild.xml" '-vs_OutDir:$(OutDir)' "-vs_ConfigurationName:$(ConfigurationName)" "-vs_ProjectName:$(ProjectName)" "-vs_TargetName:$(TargetName)" "-vs_TargetPath:$(TargetPath)" "-vs_ProjectPath:$(ProjectPath)" "-vs_ProjectFileName:$(ProjectFileName)" "-vs_TargetExt:$(TargetExt)" "-vs_TargetFileName:$(TargetFileName)" "-vs_DevEnvDir:$(DevEnvDir)" "-vs_TargetDir:$(TargetDir)" "-vs_ProjectDir:$(ProjectDir)" "-vs_SolutionFileName:$(SolutionFileName)" "-vs_SolutionPath:$(SolutionPath)" "-vs_SolutionDir:$(SolutionDir)" "-vs_SolutionName:$(SolutionName)" "-vs_PlatformName:$(PlatformName)" "-vs_ProjectExt:$(ProjectExt)" "-vs_SolutionExt:$(SolutionExt)"
}
