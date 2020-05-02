// <copyright file="CopyDirCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    public class CopyDirCommand : BuildCommandBase
    {
        public CopyDirCommand(BuildHelper buildHelper)
            : base(buildHelper)
        {
            this.BuildHelper = buildHelper;
        }

        public BuildHelper BuildHelper
        {
            get;
        }

        public override string Name => "copydir";

        public override void Execute(IDictionary<string, string> variables, XElement data)
        {
            // Debugger.Launch();
            var srcPath = data.GetValue("source");
            var destPath = data.GetValue("destination");
            if (destPath == null)
            {
                throw new XmlException($"Command: {nameof(CopyDirCommand)} value: destination  - Value not found");
            }

            if (srcPath == null)
            {
                throw new XmlException($"Command: {nameof(CopyDirCommand)} value: source  - Value not found");
            }

            srcPath = srcPath.ReplaceBuildVariableStrings(variables);
            destPath = destPath.ReplaceBuildVariableStrings(variables);

            // check if we should clear the folder first
            var value = data.GetValue("clean");
            var doClear = string.IsNullOrWhiteSpace(value) ? false : value.Trim().ToLowerInvariant() == "true";
            this.Output($"Clearing before copy ({doClear}): {destPath}");
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

            var allFiles = Directory.GetFiles(srcPath, "*.*", SearchOption.AllDirectories).Select(d => d.Remove(0, srcPath.Length));
            foreach (var file in allFiles)
            {
                var src = Path.Combine(srcPath, file);
                var dest = Path.Combine(destPath, file);

                Directory.CreateDirectory(Path.GetDirectoryName(dest));
                this.Output("Copying: " + src + " ==> " + dest);
                File.Copy(src, dest, true);
            }
        }
    }
}