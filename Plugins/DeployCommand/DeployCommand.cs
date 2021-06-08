// <copyright file="DeployCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;
    using System.IO;

    [NamedParameter("path", typeof(string), true, "The full directory path to be purged.")]
    [NamedParameter("clean", typeof(bool), false, "If true will delete contents from the destination before copying. Default is false.")]
    [NamedVariable("ProjectDir", typeof(string), true, "The full project directory path to be deployed from.")]
    [NamedVariable("OutDir", typeof(string), true, "The full output directory path to be deployed to.")]
    public class DeployCommand : ICommandPlugin
    {
        public string Name => "deploy";

        public void Run(RunCommandArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var destPath = args.GetParameter<string>("path", null);
            destPath = destPath != null ? destPath.ReplaceVariableStrings(args.Variables) : null;
            if (destPath == null)
            {
                args.Result = RunResult.Errored(new BuildException($"Command: {nameof(DeployCommand)} value: path  - Value not found"));
                return;
            }

            // check if we should clear the folder first
            var doClean = args.GetParameter("clean", false);

            args.Output($"Clearing before deploy ({doClean}): {destPath}");
            try
            {
                var di = new DirectoryInfo(destPath);
                if (doClean && di.Exists)
                {
                    foreach (var dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }

                    foreach (var file in di.EnumerateFiles())
                    {
                        file.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                args.Result = RunResult.Errored(ex);
                return;
            }

            // ensure there is a ProjectDir and OutDir variables
            if (!args.Variables.ContainsKey("ProjectDir"))
            {
                args.Result = RunResult.Errored(new MissingVariableException("Deploy command requires a 'ProjectDir' variable to run."));
                return;
            }

            if (!args.Variables.ContainsKey("OutDir"))
            {
                args.Result = RunResult.Errored(new MissingVariableException("Deploy command requires a 'OutDir' variable to run."));
                return;
            }

            var projectDir = "$(ProjectDir)".ReplaceVariableStrings(args.Variables);
            var outDir = "$(OutDir)".ReplaceVariableStrings(args.Variables);
            var srcPath = Path.Combine(projectDir, outDir);
            try
            {
                if (Directory.Exists(srcPath))
                {
                    var allFiles = Directory.GetFiles(srcPath, "*.*", SearchOption.AllDirectories);
                    foreach (var file in allFiles)
                    {
                        var filePath = file.Substring(srcPath.Length).Trim();
                        filePath = filePath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                        var src = Path.Combine(srcPath, filePath);
                        var dest = Path.Combine(destPath, filePath);

                        Directory.CreateDirectory(Path.GetDirectoryName(dest));
                        args.Output("Deploying: " + src + " ==> " + dest);
                        File.Copy(src, dest, true);
                    }
                }
            }
            catch (Exception ex)
            {
                args.Result = RunResult.Errored(ex);
                return;
            }

            args.Result = RunResult.Sucessful();
        }
    }
}