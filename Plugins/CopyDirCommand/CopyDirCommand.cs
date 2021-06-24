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

            // check if we should clear the folder first
            var doClear = args.GetParameter("clean", false);

            this.status?.Report($"Clearing before copy ({doClear}): {destPath}");
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

                var searchOption = copySubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                var allFiles = Directory.GetFiles(srcPath, "*.*", searchOption).Select(d => d.Remove(0, srcPath.Length + 1)).ToArray();
                for (var index = 0; index < allFiles.Length; index++)
                {
                    var file = allFiles[index];
                    var src = Path.Combine(srcPath, file);
                    var dest = Path.Combine(destPath, file);

                    var directoryName = Path.GetDirectoryName(dest);
                    directoryName = Path.Combine(destPath, directoryName);
                    Directory.CreateDirectory(directoryName);
                    var progress = ((float)index / allFiles.Length) * 100;
                    this.status?.ReportProgress("Copying: " + src + " ==> " + dest, progress);
                    File.Copy(src, dest, true);
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