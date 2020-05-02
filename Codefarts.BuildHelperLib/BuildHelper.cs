// <copyright file="BuildHelper.cs" company="Codefarts">
// Copyright (c) Codefarts
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    public class BuildHelper
    {
        public event OutputEventHandler OutputMessage;

        public void Output(string message, params object[] args)
        {
            this.OnOutputMessage(string.Format(message, args));
        }

        public void Build(string buildFile)
        {
            // Debugger.Launch();
            this.Output("{0} START BUILD {1}", new string('#', 6), new string('#', 25));

            // read build file
            IDictionary<string, string> variables;
            XElement root;
            if (!this.TryReadBuildFile(buildFile, out variables, out root))
            {
                this.Output("{0} END BUILD {1}", new string('#', 6), new string('#', 23));
                Environment.ExitCode = 1;
                return;
            }

            this.Output("Performing " + variables["BuildEvent"] + " build event ...");

            // load commands
            var commands = new IBuildCommand[]
                {
                    new DeployCommand(this),
                    new CopyDirCommand(this),
                    new ExcludeReferenceCommand(this),
                    new RestoreReferencesCommand(this),
                };

            // process elements
            foreach (var element in root.Elements())
            {
                var com = commands.FirstOrDefault(c => c.Name.Equals(element.Name.LocalName, StringComparison.OrdinalIgnoreCase));
                if (com == null)
                {
                    continue;
                }

                com.Execute(variables, element);
            }

            this.Output("{0} END BUILD {1}", new string('#', 6), new string('#', 23));
        }

        private bool TryReadBuildFile(string buildFile, out IDictionary<string, string> variables, out XElement root)
        {
            // var args = Environment.GetCommandLineArgs();
            // var buildFile = args.FirstOrDefault(x => x.StartsWith("-bf:"));
            variables = null;

            // ensure file exists
            if (buildFile == null)
            {
                this.Output("Build file not specified.");
                root = null;
                return false;
            }

            var buildFileInfo = new FileInfo(buildFile);

            // ensure file exists
            if (!buildFileInfo.Exists)
            {
                this.Output("Missing build file: " + buildFileInfo.FullName);
                root = null;
                return false;
            }

            this.Output("Reading build file: {0}", buildFileInfo.FullName);

            // read file
            XDocument doc;
            try
            {
                doc = XDocument.Parse(File.ReadAllText(buildFileInfo.FullName));
            }
            catch (Exception ex)
            {
                this.Output("Error reading file: " + buildFileInfo.FullName);
                this.Output(ex.Message);
                root = null;
                return false;
            }

            if (!doc.Root.Name.LocalName.Equals("build", StringComparison.OrdinalIgnoreCase))
            {
                this.Output("Error parsing build file: " + buildFileInfo.FullName);
                this.Output("Root node not 'build'.");
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
            this.Output("... Success!");
            root = doc.Root;
            return true;
        }

        protected virtual void OnOutputMessage(string message)
        {
            var handler = this.OutputMessage;
            if (handler != null)
            {
                handler(this, new OutputEventArgs(message));
            }
        }
    }
}
