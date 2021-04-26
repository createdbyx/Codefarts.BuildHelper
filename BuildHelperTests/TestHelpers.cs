// <copyright file="TestHelpers.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace BuildHelperTests
{
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Codefarts.BuildHelper;

    public class TestHelpers
    {

        public static Node BuildCommandNode(XElement xElement, Node parent)
        {
            var node = new Node(xElement.Name.LocalName);
            foreach (var attribute in xElement.Attributes())
            {
                node.Parameters[attribute.Name.LocalName] = attribute.Value;
            }

            node.Parent = parent;
            node.Children = new ObservableCollection<Node>(xElement.Elements().Select(x => BuildCommandNode(x, node)));

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
    }
}