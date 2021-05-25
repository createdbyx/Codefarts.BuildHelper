// <copyright file="BuildHelper.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Xml.Linq;

    public class BuildHelper
    {
        private readonly BuildFileReader buildFileReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildHelper"/> class.
        /// </summary>
        public BuildHelper()
        {
            this.buildFileReader = new BuildFileReader(this);
        }

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
            if (!this.buildFileReader.TryReadBuildFile(buildFile, out variables, out root))
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
                    var ignoreConditions = buildFileCommand.GetParameter("ignoreconditions", true);
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

        private CommandData BuildCommandNode(XElement xElement, CommandData parent)
        {
            var node = new CommandData(xElement.Name.LocalName);
            foreach (var attribute in xElement.Attributes())
            {
                node.Parameters[attribute.Name.LocalName] = attribute.Value;
            }

            node.Parent = parent;
            node.Children = new ObservableCollection<CommandData>(xElement.Elements().Select(x => this.BuildCommandNode(x, node)));

            return node;
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