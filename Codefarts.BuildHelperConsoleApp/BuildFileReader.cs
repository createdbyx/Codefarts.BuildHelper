// <copyright file="BuildFileReader.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelperConsoleApp
{
    using System;
    using System.IO;
    using System.Xml.Linq;
    using Codefarts.BuildHelper;

    public class BuildFileReader
    {
        private IStatusReporter status;

        public BuildFileReader(IStatusReporter status)
        {
            this.status = status ?? throw new ArgumentNullException(nameof(status));
        }

        public bool TryReadBuildFile(string buildFile, out XElement root)
        {
            // ensure file exists
            if (buildFile == null)
            {
                this.status.Report("Build file not specified.");
                root = null;
                return false;
            }

            var buildFileInfo = new FileInfo(buildFile);

            // ensure file exists
            if (!buildFileInfo.Exists)
            {
                this.status.Report("Missing build file: " + buildFileInfo.FullName);
                root = null;
                return false;
            }

            this.status.Report("Reading build file: {0}", buildFileInfo.FullName);

            // read file
            XDocument doc;
            try
            {
                doc = XDocument.Parse(File.ReadAllText(buildFileInfo.FullName));
            }
            catch (Exception ex)
            {
                this.status.Report("Error reading file: " + buildFileInfo.FullName);
                this.status.Report(ex.Message);
                root = null;
                return false;
            }

            if (!doc.Root.Name.LocalName.Equals("build", StringComparison.OrdinalIgnoreCase))
            {
                this.status.Report("Error parsing build file: " + buildFileInfo.FullName);
                this.status.Report("Root node not 'build'.");
                root = null;
                return false;
            }

            this.status.Report("... Success!");
            root = doc.Root;
            return true;
        }
    }
}