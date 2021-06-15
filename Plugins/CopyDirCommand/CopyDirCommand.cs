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
            var srcPath = args.GetParameter<string>("source", null);
            var destPath = args.GetParameter<string>("destination", null);
            //var message = args.Element.GetAttributeValue("message");

            if (destPath == null)
            {
                args.Result = RunResult.Errored(
                    new MissingParameterException($"Command: {nameof(CopyDirCommand)} value: destination  - Value not found"));
            }

            if (srcPath == null)
            {
                args.Result = RunResult.Errored(new MissingParameterException($"Command: {nameof(CopyDirCommand)} value: source  - Value not found"));
            }

            srcPath = srcPath.ReplaceVariableStrings(args.Variables);
            destPath = destPath.ReplaceVariableStrings(args.Variables);

            //if (!string.IsNullOrWhiteSpace(message))
            //{
            //    message = message.ReplaceBuildVariableStrings(args.Variables);
            //    args.Output($"Message: {message}");
            //}

            // check if we should clear the folder first
            var doClear = args.GetParameter("clean", false);
            // var doClear = string.IsNullOrWhiteSpace(value) ? false : value.Trim().ToLowerInvariant() == "true";
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

                var allFiles = Directory.GetFiles(srcPath, "*.*", SearchOption.AllDirectories).Select(d => d.Remove(0, srcPath.Length)).ToArray();
                for (var index = 0; index < allFiles.Length; index++)
                {
                    var file = allFiles[index];
                    var src = Path.Combine(srcPath, file);
                    var dest = Path.Combine(destPath, file);

                    Directory.CreateDirectory(Path.GetDirectoryName(dest));
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