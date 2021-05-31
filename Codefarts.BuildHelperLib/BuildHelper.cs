﻿// <copyright file="BuildHelper.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BuildHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildHelper"/> class.
        /// </summary>
        public BuildHelper()
        {
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

        public void Run(
            IEnumerable<CommandData> commands,
            IDictionary<string, object> variables,
            IEnumerable<IBuildCommand> commandPlugins)
        {
            commands = commands ?? Enumerable.Empty<CommandData>();

            variables = variables ?? new Dictionary<string, object>();

            commandPlugins = commandPlugins ?? Enumerable.Empty<IBuildCommand>();

            var buildEventValue = variables.GetValue<string>("BuildEvent", null);

            this.OutputHeader($"START {buildEventValue} BUILD");

            // process file elements
            foreach (var command in commands)
            {
                // find the first plugin with the matching name
                var plugin = commandPlugins.FirstOrDefault(c => c.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase));
                if (plugin == null)
                {
                    continue;
                }

                // setup executing args
                var executeCommandArgs = new RunCommandArgs(
                    msg => this.Output(msg),
                    variables,
                    command,
                    this);
                try
                {
                    // check if the command has an attached message and if so output the message before executing
                    var message = command.GetParameter("message", string.Empty);
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        message = message.ReplaceVariableStrings(variables);
                        this.Output($"Message: {message}");
                    }

                    // check to ignore conditions
                    var ignoreConditions = command.GetParameter("ignoreconditions", true);
                    if (!ignoreConditions)
                    {
                        // check type of conditions
                        var allConditions = command.GetParameter("allconditions", true);
                        // var allConditions = true;
                        //if (conditionsValue != null && !bool.TryParse(conditionsValue, out allConditions))
                        //{
                        //    throw new ArgumentOutOfRangeException($"'{allConditions}' attribute exists but it's value could not be parsed as a bool value.");
                        //}

                        // check conditions
                        if (!command.SatifiesConditions(variables, allConditions))
                        {
                            this.Output($"Conditions not satisfied for command '{command.Name}'.");
                            return;
                        }
                    }

                    // execute the command plugin
                    plugin.Run(executeCommandArgs);
                }
                catch (Exception ex)
                {
                    this.Output($"Command {plugin.Name} threw an exception.");
                    this.Output(ex.ToString());
                }
            }

            this.OutputHeader($"END {buildEventValue} BUILD");
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