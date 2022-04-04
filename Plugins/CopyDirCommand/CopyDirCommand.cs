// <copyright file="CopyDirCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

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
            this.status = status ?? throw new ArgumentNullException(nameof(status));
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

            // check if we should clear the folder first
            var doClear = args.GetParameter("clean", false);

            this.status.Report($"Clearing before copy ({doClear}): {destPath}");
            try
            {
                var di = new DirectoryInfo(destPath);
                if (doClear && di.Exists)
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

                var copySubFolders = args.GetParameter("subfolders", true);
                // var relativePaths = args.GetParameter("relativepaths", false);

                var searchOption = copySubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                var allFiles = Directory.GetFiles(srcPath, "*.*", searchOption)
                                        // .Select(d => relativePaths ? d.Remove(0, srcPath.Length + 1) : d)
                                        .ToArray();
                var variables = new VariablesDictionary(args.Variables);
                for (var index = 0; index < allFiles.Length; index++)
                {
                    var src = allFiles[index];
                    var dest = Path.Combine(destPath, src.Substring(srcPath.Length + 1));

                    var directoryName = Path.GetDirectoryName(dest) ?? dest;

                    // report progress
                    var progress = ((float)index / allFiles.Length) * 100;
                    this.status.ReportProgress("Copying: " + src + " ==> " + dest, progress); // check conditionals

                    // check to ignore conditions
                    if (ignoreConditions)
                    {
                        // do copy
                        Directory.CreateDirectory(directoryName);
                        File.Copy(src, dest, true);
                        continue;
                    }

                    // check if conditions are NOT satisfied
                    if (!args.Command.SatifiesConditions(variables, allConditions, src))
                    {
                        // do copy
                        Directory.CreateDirectory(directoryName);
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