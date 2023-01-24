// <copyright file="CopyDirCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Codefarts.BuildHelper.Exceptions;

namespace Codefarts.BuildHelper
{
    using System;
    using System.IO;
    using System.Linq;

    [NamedParameter("source", typeof(string), true, "The source folder that will be copied.")]
    [NamedParameter("destination", typeof(string), true, "The destination folder where files and folder will be copied to.")]
    [NamedParameter("clean", typeof(bool), false, "If true will delete contents from the destination before copying. Default is false.")]
    [NamedParameter("subfolders", typeof(bool), false, "If true will copy subfolders as well. Default is true.")]
    [NamedParameter("allconditions", typeof(bool), false, "Specifies weather or not all conditions must be satisfied. Default is false.")]
    [NamedParameter("ignoreconditions", typeof(bool), false, "Specifies weather to ignore conditions. Default is false.")]
    [NamedParameter("test", typeof(bool), false, "Specifies weather to run in test mode. Default is false.")]
    //  [NamedParameter("relativepaths", typeof(bool), false,
    //                  "Specifies weather condition checks will compare against relative paths or full file paths. Default is false.")]
    public class CopyDirCommand : ICommandPlugin
    {
        private IStatusReporter status;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyDirCommand"/> class.
        /// </summary>
        public CopyDirCommand(IStatusReporter status)
        {
            this.status = status; 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyDirCommand"/> class.
        /// </summary>
        public CopyDirCommand()
        {
        }

        public string Name => "copydir";

        public void Run(RunCommandArgs args)
        {
            //  Debugger.Launch();
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var srcPath = args.GetParameter<string>("source", null);
            var destPath = args.GetParameter<string>("destination", null);

            if (string.IsNullOrWhiteSpace(destPath))
            {
                args.Result = RunResult.Errored(new MissingParameterException("destination"));
                return;
            }

            if (string.IsNullOrWhiteSpace(srcPath))
            {
                args.Result = RunResult.Errored(new MissingParameterException("source"));
                return;
            }

            srcPath = srcPath.ReplaceVariableStrings(args.Variables);
            destPath = destPath.ReplaceVariableStrings(args.Variables);

            // validate src and dest paths
            if (srcPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                args.Result = RunResult.Errored(
                    new MissingParameterException("source", "Contains invalid path characters after replacing variables."));
                return;
            }

            if (destPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                args.Result = RunResult.Errored(
                    new MissingParameterException("destination", "Contains invalid path characters after replacing variables."));
                return;
            }

            var allConditions = args.GetParameter("allconditions", false);
            var ignoreConditions = args.GetParameter("ignoreconditions", false);
            var isTesting = args.GetParameter<bool>("test", false);
            var testText = isTesting ? "(In Test Mode) " : string.Empty;

            // check if we should clear the folder first
            var doClear = args.GetParameter("clean", false);

            this.status?.Report($"{testText}Clearing before copy ({doClear}): {destPath}");
            try
            {
                var di = new DirectoryInfo(destPath);
                if (doClear && di.Exists)
                {
                    foreach (var file in di.EnumerateFiles())
                    {
                        if (!isTesting)
                        {
                            file.Delete();
                        }
                    }

                    foreach (var dir in di.GetDirectories())
                    {
                        if (!isTesting)
                        {
                            dir.Delete(true);
                        }
                    }
                }

                var copySubFolders = args.GetParameter("subfolders", true);
                // var relativePaths = args.GetParameter("relativepaths", false);

                var searchOption = copySubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                srcPath = Path.GetFullPath(srcPath);
                var allFiles = Directory.GetFiles(srcPath, "*.*", searchOption)
                                        // .Select(d => relativePaths ? d.Remove(0, srcPath.Length + 1) : d)
                                        .ToArray();
                var variables = new VariablesDictionary(args.Variables);
                var pathChars = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
                for (var index = 0; index < allFiles.Length; index++)
                {
                    var src = allFiles[index];
                    var path = src.Remove(0, srcPath.Length).TrimStart().TrimStart(pathChars);
                    var dest = Path.GetFullPath(Path.Combine(destPath, path));
                    var directoryName = Path.GetDirectoryName(dest) ?? dest;

                    // report progress
                    var progress = (float)Math.Round(((float)index / allFiles.Length) * 100, 2);

                    // check to ignore conditions
                    if (ignoreConditions)
                    {
                        // do copy
                        this.status?.ReportProgress(testText + "Copying: " + src + " ==> " + dest, progress); // check conditionals
                        if (!isTesting)
                        {
                            Directory.CreateDirectory(directoryName);
                            File.Copy(src, dest, true);
                        }

                        continue;
                    }

                    // check if conditions are satisfied
                    if (args.Command.SatifiesConditions(variables, allConditions, src))
                    {
                        this.status?.ReportProgress(testText + "Copying: " + src + " ==> " + dest, progress); // check conditionals
                        // do copy
                        if (!isTesting)
                        {
                            Directory.CreateDirectory(directoryName);
                            File.Copy(src, dest, true);
                        }
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