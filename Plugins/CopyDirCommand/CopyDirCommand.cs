// <copyright file="CopyDirCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    public class CopyDirCommand : IBuildCommand
    {
        public string Name => "copydir";

        public void Execute(ExecuteCommandArgs args)
        {
            var srcPath = args.GetParameter<string>("source");
            var destPath = args.GetParameter<string>("destination");
            //var message = args.Element.GetAttributeValue("message");

            if (destPath == null)
            {
                throw new XmlException($"Command: {nameof(CopyDirCommand)} value: destination  - Value not found");
            }

            if (srcPath == null)
            {
                throw new XmlException($"Command: {nameof(CopyDirCommand)} value: source  - Value not found");
            }

            srcPath = srcPath.ReplaceBuildVariableStrings(args.Variables);
            destPath = destPath.ReplaceBuildVariableStrings(args.Variables);

            //if (!string.IsNullOrWhiteSpace(message))
            //{
            //    message = message.ReplaceBuildVariableStrings(args.Variables);
            //    args.Output($"Message: {message}");
            //}

            // check if we should clear the folder first
            var doClear = args.GetParameter("clean", false);
            // var doClear = string.IsNullOrWhiteSpace(value) ? false : value.Trim().ToLowerInvariant() == "true";
            args.Output($"Clearing before copy ({doClear}): {destPath}");
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
                args.Output("Copying: " + src + " ==> " + dest);
                File.Copy(src, dest, true);
            }
        }
    }
}