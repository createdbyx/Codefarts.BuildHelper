// <copyright file="DeployCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System;

namespace Codefarts.BuildHelper
{
    using System.IO;
    using System.Linq;

    public class DeployCommand : IBuildCommand
    {
        public string Name => "deploy";

        public void Execute(ExecuteCommandArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (args.Variables == null)
            {
                throw new BuildException("Variables dictionary is null.");
            }

            var destPath = args.GetParameter<string>("path", null);
            destPath = destPath != null ? destPath.ReplaceVariableStrings(args.Variables) : null;
            if (destPath == null)
            {
                throw new BuildException($"Command: {nameof(DeployCommand)} value: path  - Value not found");
            }

            // check if we should clear the folder first
            var doClean = args.GetParameter("clean", false);

            args.Output($"Clearing before deploy ({doClean}): {destPath}");
            var di = new DirectoryInfo(destPath);
            if (doClean && di.Exists)
            {
                foreach (var file in di.EnumerateFiles())
                {
                    file.Delete();
                }

                foreach (var dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }

            // ensure there is a ProjectDir and OutDir variables
            if (!args.Variables.ContainsKey("ProjectDir"))
            {
                throw new BuildException("Deploy command requires a 'ProjectDir' variable to run.");
            }

            if (!args.Variables.ContainsKey("OutDir"))
            {
                throw new BuildException("Deploy command requires a 'OutDir' variable to run.");
            }

            var srcPath = Path.Combine("$(ProjectDir)".ReplaceVariableStrings(args.Variables),
                                       "$(OutDir)".ReplaceVariableStrings(args.Variables));
            if (Directory.Exists(srcPath))
            {
                var allFiles = Directory.GetFiles(srcPath, "*.*", SearchOption.AllDirectories);//.Select(d => Path.GetFileName(d));
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
    }
}