// <copyright file="XmlCommandFileReader.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using Codefarts.DependencyInjection;

namespace Codefarts.BuildHelperConsoleApp;

using System;
using System.IO;
using System.Xml.Linq;
using Codefarts.BuildHelper;

public class XmlCommandFileReader : ICommandImporter
{
    private IStatusReporter status;
    private readonly IDependencyInjectionProvider ioc;

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlCommandFileReader"/> class.
    /// </summary>
    /// <param name="ioc">A reference to a <see cref="IDependencyInjectionProvider"/>.</param>
    /// <exception cref="ArgumentNullException">If the <param name="ioc"/> is null.</exception>
    public XmlCommandFileReader(IDependencyInjectionProvider ioc)
    {
        this.ioc = ioc ?? throw new ArgumentNullException(nameof(ioc));
        try
        {
            this.status = ioc.Resolve<IStatusReporter>();
        }
        catch
        {
        }
    }

    private bool TryReadBuildFile(string buildFile, out CommandData data)
    {
        // ensure file exists
        if (string.IsNullOrWhiteSpace(buildFile))
        {
            this.status?.Report("Build file not specified.");
            data = null;
            return false;
        }

        var buildFileInfo = new FileInfo(buildFile);

        // ensure file exists
        if (!buildFileInfo.Exists)
        {
            this.status?.Report($"Missing build file: {buildFileInfo.FullName}");
            data = null;
            return false;
        }

        this.status?.Report($"Reading build file: {buildFileInfo.FullName}");

        // read file
        XDocument doc;
        try
        {
            doc = XDocument.Parse(File.ReadAllText(buildFileInfo.FullName));
        }
        catch (Exception ex)
        {
            this.status?.Report($"Error reading file: {buildFileInfo.FullName}");
            this.status?.Report(ex.Message);
            data = null;
            return false;
        }

        if (!doc.Root.Name.LocalName.Equals("build", StringComparison.OrdinalIgnoreCase))
        {
            this.status?.Report($"Error parsing build file: {buildFileInfo.FullName}");
            this.status?.Report("Root node not 'build'.");
            data = null;
            return false;
        }

        this.status?.Report("... Success!");
        data = BuildCommandNode(doc.Root, null);
        return true;
    }

    private CommandData BuildCommandNode(XElement xElement, CommandData parent)
    {
        var node = this.ioc.Resolve<CommandData>();
        node.Name = xElement.Name.LocalName;
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

    public RunResult Run()
    {
        // ? how do I get the file file anme

        IConfigurationProvider config;
        try
        {
            config = this.ioc.Resolve<IConfigurationProvider>();
        }
        catch
        {
            return RunResult.Errored(new NullReferenceException($"No {nameof(IConfigurationProvider)} is available."));
        }

        var buildFile = config.GetValue("filename", default(string));

        if (!this.TryReadBuildFile(buildFile, out var rootCommand))
        {
            return RunResult.Errored(new IOException($"Problem reading file '{buildFile}'."));
        }

        return RunResult.Sucessful(rootCommand);
    }
}