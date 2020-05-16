// <copyright file="DeployCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml;

    public class DeployCommand : IBuildCommand
    {
        public string Name => "deploy";

        public void Execute(ExecuteCommandArgs args)
        {
            var value = args.Element.GetValue("path");
            var destPath = value != null ? value.ReplaceBuildVariableStrings(args.Variables) : null;
            if (destPath == null)
            {
                throw new XmlException($"Command: {nameof(DeployCommand)} value: path  - Value not found");
            }

            var message = args.Element.GetValue("message");

            // check if we should clear the folder first
            value = args.Element.GetValue("clean");
            var doClear = string.IsNullOrWhiteSpace(value) ? false : value.Trim().ToLowerInvariant() == "true";

            // check type of conditions
            var conditionsValue = args.Element.GetValue("allconditions");
            var allConditions = true;
            if (conditionsValue != null && !bool.TryParse(conditionsValue, out allConditions))
            {
                throw new ArgumentOutOfRangeException($"'{allConditions}' attribute exists but it's value could not be parsed as a bool value.");
            }

            // check conditions
            if (!args.Element.SatifiesConditions(args.Variables, allConditions))
            {
                args.Output("Conditions not satisfied.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                message = message.ReplaceBuildVariableStrings(args.Variables);
                args.Output($"Message: {message}");
            }

            args.Output($"Clearing before deploy ({doClear}): {destPath}");
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

            var srcPath = "$(ProjectDir)$(OutDir)".ReplaceBuildVariableStrings(args.Variables);
            var allFiles = Directory.GetFiles(srcPath, "*.*", SearchOption.AllDirectories).Select(d => Path.GetFileName(d));
            foreach (var file in allFiles)
            {
                var src = Path.Combine(srcPath, file);
                var dest = Path.Combine(destPath, file);

                Directory.CreateDirectory(Path.GetDirectoryName(dest));
                args.Output("Deploying: " + src + " ==> " + dest);
                File.Copy(src, dest, true);
            }
        }
    }
}