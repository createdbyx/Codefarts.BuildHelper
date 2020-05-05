// <copyright file="PurgeCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

//using System.Diagnostics;

namespace Codefarts.BuildHelper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    public class PurgeCommand : BuildCommandBase
    {
        public PurgeCommand(Action<string> writeOutput)
            : base(writeOutput)
        {
        }

        public override string Name => "purge";

        public override void Execute(IDictionary<string, string> variables, XElement data)
        {
            // Debugger.Launch();
            var srcPath = data.GetValue("path");
            if (srcPath == null)
            {
                throw new XmlException($"Command: {nameof(PurgeCommand)} value: path  - Value not found");
            }

            srcPath = srcPath.ReplaceBuildVariableStrings(variables);

            var message = data.GetValue("message");

            // check type of purge
            var typeValue = data.GetValue("type");
            typeValue = typeValue == null ? typeValue : typeValue.ToLowerInvariant().Trim();
            switch (typeValue)
            {
                case null:
                    throw new ArgumentNullException($"'{typeValue}' is missing.");

                case "files":
                case "folders":
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"'{typeValue}' attribute exists but it value could not be understood.");
            }

            // check type of conditions
            var conditionsValue = data.GetValue("allconditions");
            var allConditions = true;
            if (conditionsValue != null && !bool.TryParse(conditionsValue, out allConditions))
            {
                throw new ArgumentOutOfRangeException($"'{allConditions}' attribute exists but it's value could not be parsed as a bool value.");
            }

            // check to use full file paths 
            var fullPathsValue = data.GetValue("fullpaths");
            var fullPaths = false;
            if (fullPathsValue != null && !bool.TryParse(fullPathsValue, out fullPaths))
            {
                throw new ArgumentOutOfRangeException($"'{fullPaths}' attribute exists but it's value could not be parsed as a bool value.");
            }

            // check if we should clear subfolders as well
            var subFoldersValue = data.GetValue("subfolders");
            var subfolders = true;
            if (subFoldersValue != null && !bool.TryParse(subFoldersValue, out subfolders))
            {
                throw new ArgumentOutOfRangeException($"'{subfolders}' attribute exists but it's value could not be parsed as a bool value.");
            }

            var di = new DirectoryInfo(srcPath);
            if (!di.Exists)
            {
                this.Output($"Purging folder failed! (Reason: Does not exist!): {srcPath}");
                return;
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                message = message.ReplaceBuildVariableStrings(variables);
                this.Output($"Message: {message}");
            }

            this.Output($"(Sub Folders->{subfolders}): {srcPath}");

            var getEntries = new Func<string, IEnumerable<string>>(p =>
            {
                if (typeValue == "files")
                {
                    return Directory.GetFiles(srcPath, "*.*", subfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);//.Select(d => fullPaths ? d : Path.GetFileName(d));
                }

                return Directory.GetDirectories(srcPath, "*.*", subfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);//.Select(d => fullPaths ? d : Path.GetFileName(d));
            });

            var deleteEntry = new Action<string>(p =>
            {
                if (typeValue == "files")
                {
                    File.Delete(p);
                    return;
                }

                Directory.Delete(p, true);
            });

            // TODO: need option to specify weather or not $(PurgeFile) is just the filename or full path
            var allEntries = getEntries(srcPath);
            var modifiedVars = new Dictionary<string, string>(variables);
            foreach (var file in allEntries)
            {
                var src = fullPaths ? file : Path.GetFileName(file);
                modifiedVars["PurgeEntry"] = src;

                if (data.SatifiesConditions(modifiedVars, allConditions))
                {
                    // TODO: need ability to purge folder as well
                    this.Output("Purging: " + file);
                    deleteEntry(file);
                }
            }
        }
    }
}