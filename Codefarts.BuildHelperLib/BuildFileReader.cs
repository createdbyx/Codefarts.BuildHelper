// <copyright file="BuildFileReader.cs" company="Codefarts">
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
    using System.Xml.Linq;

    internal class BuildFileReader
    {
        private BuildHelper buildHelper;

        public BuildFileReader(BuildHelper buildHelper)
        {
            this.buildHelper = buildHelper ?? throw new ArgumentNullException(nameof(buildHelper));
        }

        public bool TryReadBuildFile(string buildFile, out IDictionary<string, string> variables, out XElement root)
        {
            // var args = Environment.GetCommandLineArgs();
            // var buildFile = args.FirstOrDefault(x => x.StartsWith("-bf:"));
            variables = null;

            // ensure file exists
            if (buildFile == null)
            {
                this.buildHelper.Output("Build file not specified.");
                root = null;
                return false;
            }

            var buildFileInfo = new FileInfo(buildFile);

            // ensure file exists
            if (!buildFileInfo.Exists)
            {
                this.buildHelper.Output("Missing build file: " + buildFileInfo.FullName);
                root = null;
                return false;
            }

            this.buildHelper.Output("Reading build file: {0}", buildFileInfo.FullName);

            // read file
            XDocument doc;
            try
            {
                doc = XDocument.Parse(File.ReadAllText(buildFileInfo.FullName));
            }
            catch (Exception ex)
            {
                this.buildHelper.Output("Error reading file: " + buildFileInfo.FullName);
                this.buildHelper.Output(ex.Message);
                root = null;
                return false;
            }

            if (!doc.Root.Name.LocalName.Equals("build", StringComparison.OrdinalIgnoreCase))
            {
                this.buildHelper.Output("Error parsing build file: " + buildFileInfo.FullName);
                this.buildHelper.Output("Root node not 'build'.");
                root = null;
                return false;
            }

            // find variables node
            var varNodes = doc.Root.Elements("buildvaribles");
            var varNode = varNodes.SelectMany(r => r.Elements());

            // parse data from file
            variables = varNode.ToDictionary(k => k.Name.LocalName, v => v.Value);

            foreach (var node in varNodes)
            {
                node.Remove();
            }

            if (varNodes != null)
            {
                varNodes.Remove();
            }

            variables["Application"] = Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            this.buildHelper.Output("... Success!");
            root = doc.Root;
            return true;
        }
    }
}