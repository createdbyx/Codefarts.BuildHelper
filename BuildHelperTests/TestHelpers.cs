// <copyright file="TestHelpers.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System;
using System.Linq;

namespace BuildHelperTests
{
    using System.IO;
    using System.Xml.Linq;
    using Codefarts.BuildHelper;

    public class TestHelpers
    {
        public static CommandData BuildCommandNode(XElement xElement, CommandData parent)
        {
            var node = new CommandData(xElement.Name.LocalName);
            foreach (var attribute in xElement.Attributes())
            {
                node.Parameters[attribute.Name.LocalName] = attribute.Value;
            }

            node.Parent = parent;
            foreach (var childItem in xElement.Elements())
            {
                var newData = BuildCommandNode(childItem, node);
                node.Children.Add(newData);
            }

            return node;
        }

        public static void BuildFoldersAndFiles(string path)
        {
            Directory.CreateDirectory(path);
            File.WriteAllText(Path.Combine(path, "File1.txt"), "File1Data");
            File.WriteAllText(Path.Combine(path, "File2.xml"), "File2Data");
            File.WriteAllText(Path.Combine(path, "System.File3.dat"), "File3Data");
            Directory.CreateDirectory(Path.Combine(path, "SubFolder"));
            Directory.CreateDirectory(Path.Combine(path, "SubFolder", "Sub2"));
            File.WriteAllText(Path.Combine(path, "SubFolder", "Microsoft.File4.db"), "File4Data");
            File.WriteAllText(Path.Combine(path, "SubFolder", "Sub2", "Taxi.File5.pdb"), "File5Data");

            var sourceFolder = Path.Combine(path, "Source");
            Directory.CreateDirectory(sourceFolder);
            File.WriteAllText(Path.Combine(sourceFolder, "file1.txt"), "File1 contents");
        }
        
        public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles().Where(x => x.Extension.Equals(".dll", StringComparison.InvariantCultureIgnoreCase)))
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
    }
}