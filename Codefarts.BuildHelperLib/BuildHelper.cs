// <copyright file="BuildHelper.cs" company="Codefarts">
// Copyright (c) Codefarts
// </copyright>

//using System.Diagnostics;

using System.Collections.ObjectModel;

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

        public void OutputHeader(string message, params object[] args)
        {
            var formattedString = string.Format(message, args);
            var maxLen = Math.Max(formattedString.Length + 10, 100);
            var headPartLen = (maxLen - (formattedString.Length + 2)) / 2;
            var headerChars = new string('#', headPartLen);
            this.Output(string.Format($"{headerChars} {message} {headerChars}", args));
        }

        public void Build(string buildFile, IEnumerable<IBuildCommand> commandPlugins)
        {
            if (commandPlugins == null)
            {
                commandPlugins = Enumerable.Empty<IBuildCommand>();
            }

            // Debugger.Launch();

            // read build file
            IDictionary<string, string> variables;
            XElement root;
            if (!this.TryReadBuildFile(buildFile, out variables, out root))
            {
                this.Output($"ERROR: Reading Build File. {buildFile}");
                Environment.ExitCode = 1;
                return;
            }

            string buildEventValue;
            variables.TryGetValue("BuildEvent", out buildEventValue);

            this.OutputHeader($"START {buildEventValue} BUILD");

            var buildFileCommands = root.Elements().Select(x => this.BuildCommandNode(x, null));

            // process file elements
            foreach (var buildFileCommand in buildFileCommands)
            {
                // find the first plugin with the matching name
                var plugin = commandPlugins.FirstOrDefault(c => c.Name.Equals(buildFileCommand.Name, StringComparison.OrdinalIgnoreCase));
                if (plugin == null)
                {
                    continue;
                }

                // setup executing args
                var executeCommandArgs = new ExecuteCommandArgs(
                    msg => this.Output(msg),
                    variables,
                    buildFileCommand);
                try
                {
                    // check if the command has an attached message and if so output the message before executing
                    var message = buildFileCommand.GetParameter("message", string.Empty);
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        message = message.ReplaceBuildVariableStrings(variables);
                        this.Output($"Message: {message}");
                    }

                    // check to ignore conditions
                    var ignoreConditions = buildFileCommand.GetParameter("ignoreconditions", false);
                    if (!ignoreConditions)
                    {
                        // check type of conditions
                        var allConditions = buildFileCommand.GetParameter("allconditions", true);
                        // var allConditions = true;
                        //if (conditionsValue != null && !bool.TryParse(conditionsValue, out allConditions))
                        //{
                        //    throw new ArgumentOutOfRangeException($"'{allConditions}' attribute exists but it's value could not be parsed as a bool value.");
                        //}

                        // check conditions
                        if (!buildFileCommand.SatifiesConditions(variables, allConditions))
                        {
                            this.Output($"Conditions not satisfied for command '{buildFileCommand.Name}'.");
                            return;
                        }
                    }

                    // execute the command plugin
                    plugin.Execute(executeCommandArgs);
                }
                catch (Exception ex)
                {
                    this.Output($"Command {plugin.Name} threw an exception.");
                    this.Output(ex.ToString());
                }
            }

            this.OutputHeader($"END {buildEventValue} BUILD");
        }

        private Node BuildCommandNode(XElement xElement, Node parent)
        {
            var node = new Node(xElement.Name.LocalName);
            foreach (var attribute in xElement.Attributes())
            {
                node.Parameters[attribute.Name.LocalName] = attribute.Value;
            }

            node.Parent = parent;
            node.Children = new ObservableCollection<Node>(xElement.Elements().Select(x => this.BuildCommandNode(x, node)));

            return node;
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
