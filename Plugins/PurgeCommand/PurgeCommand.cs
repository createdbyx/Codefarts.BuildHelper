// <copyright file="PurgeCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using Codefarts.BuildHelper.Exceptions;

namespace Codefarts.BuildHelper
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    [NamedParameter("path", typeof(string), true, "The full directory path to be purged.")]
    [NamedParameter("type", typeof(string), true, "The type of items to e purged. 'files' or 'folders'.")]
    [NamedParameter("allconditions", typeof(bool), false, "Specifies weather or not all conditions must be satisfied. Default is true.")]
    [NamedParameter("fullpaths", typeof(bool), false, "Specifies weather to use fully qualified paths. Default is false.")]
    [NamedParameter("subfolders", typeof(bool), false, "Specifies weather to purge sub folders. Default is true.")]
    [NamedParameter("ignoreconditions", typeof(bool), false, "Specifies weather to ignore conditions. Default is false.")]
    public class PurgeCommand : ICommandPlugin
    {
        private IStatusReporter status;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommand"/> class.
        /// </summary>
        public PurgeCommand(IStatusReporter status)
        {
            this.status = status ?? throw new ArgumentNullException(nameof(status));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommand"/> class.
        /// </summary>
        public PurgeCommand()
        {
        }

        public string Name => "purge";

        public void Run(RunCommandArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var srcPath = args.GetParameter<string>("path", null);
            if (string.IsNullOrWhiteSpace(srcPath))
            {
                args.Result = RunResult.Errored(new MissingParameterException("path"));
                return;
            }

            srcPath = srcPath.ReplaceVariableStrings(args.Variables);

            // check type of purge
            var typeValue = args.GetParameter<string>("type", null);
            typeValue = typeValue == null ? typeValue : typeValue.ToUpperInvariant().Trim();
            switch (typeValue)
            {
                case null:
                    args.Result = RunResult.Errored(new MissingParameterException("type"));
                    return;

                case "FILES":
                case "FOLDERS":
                    break;

                default:
                    args.Result = RunResult.Errored(new BuildException("'type' parameter exists but it value could not be understood."));
                    return;
            }

            // check type of conditions
            var allConditions = args.GetParameter("allconditions", true);

            // check to use full file paths.
            var fullPaths = args.GetParameter("fullpaths", false);

            // check if we should clear subfolders as well
            var subfolders = args.GetParameter("subfolders", true);

            // check to ignore conditions
            var ignoreConditions = args.GetParameter("ignoreconditions", false);

            var di = new DirectoryInfo(srcPath);
            if (!di.Exists)
            {
                var message = $"Purging folder failed! (Reason: Does not exist!): {srcPath}";
                this.status?.Report(message);
                args.Result = RunResult.Errored(new DirectoryNotFoundException(message));
                return;
            }

            this.status?.Report($"(Sub Folders->{subfolders}): {srcPath}");

            string[] allEntries;
            var searchOption = subfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            try
            {
                if (typeValue == "FILES")
                {
                    allEntries = Directory.GetFiles(srcPath, "*.*", searchOption);
                }
                else
                {
                    allEntries = Directory.GetDirectories(srcPath, "*.*", searchOption);
                }

                // TODO: need option to specify weather or not $(PurgeFile) is just the filename or full path
                var modifiedVars = new Dictionary<string, object>(args.Variables);
                for (var index = 0; index < allEntries.Length; index++)
                {
                    var file = allEntries[index];
                    var src = fullPaths ? file : Path.GetFileName(file);
                    modifiedVars["PurgeEntry"] = src;

                    if (!ignoreConditions)
                    {
                        if (args.Command.SatifiesConditions(modifiedVars, allConditions))
                        {
                            this.DeleteItem(file, typeValue);
                        }
                    }
                    else
                    {
                        this.DeleteItem(file, typeValue);
                    }

                    this.status?.ReportProgress(((float)index / allEntries.Length) * 100);
                }
            }
            catch (Exception ex)
            {
                args.Result = RunResult.Errored(ex);
                return;
            }

            args.Result = RunResult.Sucessful();
        }

        private void DeleteItem(string file, string typeValue)
        {
            // TODO: need ability to purge folder as well
            this.status?.Report("Purging: " + file);

            if (typeValue == "FILES")
            {
                File.Delete(file);
            }
            else
            {
                Directory.Delete(file, true);
            }
        }
    }
}