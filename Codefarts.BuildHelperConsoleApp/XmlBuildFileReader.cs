// <copyright file="XmlBuildFileReader.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Codefarts.BuildHelperConsoleApp;

using System;
using System.IO;
using System.Xml.Linq;
using Codefarts.BuildHelper;

public class XmlBuildFileReader
{
    private IStatusReporter status;

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlBuildFileReader"/> class.
    /// </summary>
    public XmlBuildFileReader()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlBuildFileReader"/> class.
    /// </summary>
    /// <param name="status">A reference to a <see cref="IStatusReporter"/>.</param>
    /// <exception cref="ArgumentNullException">If the <param name="status"/> is null.</exception>
    public XmlBuildFileReader(IStatusReporter status)
    {
        this.status = status ?? throw new ArgumentNullException(nameof(status));
    }

    public bool TryReadBuildFile(string buildFile, out IEnumerable<CommandData> data)
    {
        // ensure file exists
        if (buildFile == null)
        {
            this.status?.Report("Build file not specified.");
            data = Enumerable.Empty<CommandData>();
            return false;
        }

        var buildFileInfo = new FileInfo(buildFile);

        // ensure file exists
        if (!buildFileInfo.Exists)
        {
            this.status?.Report("Missing build file: " + buildFileInfo.FullName);
            data = Enumerable.Empty<CommandData>();
            return false;
        }

        this.status?.Report("Reading build file: {0}", buildFileInfo.FullName);

        // read file
        XDocument doc;
        try
        {
            doc = XDocument.Parse(File.ReadAllText(buildFileInfo.FullName));
        }
        catch (Exception ex)
        {
            this.status?.Report("Error reading file: " + buildFileInfo.FullName);
            this.status?.Report(ex.Message);
            data = Enumerable.Empty<CommandData>();
            return false;
        }

        if (!doc.Root.Name.LocalName.Equals("build", StringComparison.OrdinalIgnoreCase))
        {
            this.status?.Report("Error parsing build file: " + buildFileInfo.FullName);
            this.status?.Report("Root node not 'build'.");
            data = Enumerable.Empty<CommandData>();
            return false;
        }

        this.status?.Report("... Success!");
        data = doc.Root.Elements().Select(x => BuildCommandNode(x, null));
        return true;
    }

    private CommandData BuildCommandNode(XElement xElement, CommandData parent)
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
}