// <copyright file="PurgeCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

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
    public class PurgeCommand : ICommandPlugin
    {
        public string Name => "purge";

        public void Run(RunCommandArgs args)
        {
            var srcPath = args.GetParameter<string>("path", null);
            if (srcPath == null)
            {
                throw new MissingParameterException("path");
            }

            srcPath = srcPath.ReplaceVariableStrings(args.Variables);

            // var message = args.Element.GetAttributeValue("message");

            // check type of purge
            var typeValue = args.GetParameter<string>("type", null);
            typeValue = typeValue == null ? typeValue : typeValue.ToLowerInvariant().Trim();
            switch (typeValue)
            {
                case null:
                    throw new MissingParameterException("type");

                case "files":
                case "folders":
                    break;

                default:
                    throw new BuildException("'type' parameter exists but it value could not be understood.");
            }

            // check type of conditions
            var allConditions = args.GetParameter("allconditions", true);

            // check to use full file paths.
            var fullPaths = args.GetParameter("fullpaths", false);

            // check if we should clear subfolders as well
            var subfolders = args.GetParameter("subfolders", true);

            var di = new DirectoryInfo(srcPath);
            if (!di.Exists)
            {
                args.Output($"Purging folder failed! (Reason: Does not exist!): {srcPath}");
                return;
            }

            args.Output($"(Sub Folders->{subfolders}): {srcPath}");

            string[] allEntries;
            var searchOption = subfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            if (typeValue == "files")
            {
                allEntries = Directory.GetFiles(srcPath, "*.*", searchOption);
            }
            else
            {
                allEntries = Directory.GetDirectories(srcPath, "*.*", searchOption);
            }

            // TODO: need option to specify weather or not $(PurgeFile) is just the filename or full path
            // var allEntries = getEntries(srcPath);
            var modifiedVars = new Dictionary<string, object>(args.Variables);
            foreach (var file in allEntries)
            {
                var src = fullPaths ? file : Path.GetFileName(file);
                modifiedVars["PurgeEntry"] = src;

                if (args.Command.SatifiesConditions(modifiedVars, allConditions))
                {
                    // TODO: need ability to purge folder as well
                    args.Output("Purging: " + file);

                    if (typeValue == "files")
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
    }
}