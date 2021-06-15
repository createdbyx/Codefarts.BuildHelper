// <copyright file="BuildHelper.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BuildHelper //: INotifyPropertyChanged
    {
        private IStatusReporter status;
        private VariablesDictionary variables;
        private PluginCollection commandPlugins;

        public BuildHelper(IStatusReporter status)
            : this()
        {
            this.status = status ?? throw new ArgumentNullException(nameof(status));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildHelper"/> class.
        /// </summary>
        public BuildHelper()
        {
            this.Variables = new VariablesDictionary();
            this.CommandPlugins = new PluginCollection();
        }

        public PluginCollection CommandPlugins
        {
            get
            {
                return this.commandPlugins;
            }

            private set
            {
                var currentValue = this.commandPlugins;
                if (currentValue != value)
                {
                    this.commandPlugins = value;
                    // this.OnPropertyChanged(nameof(this.CommandPlugins));
                }
            }
        }

        public VariablesDictionary Variables
        {
            get
            {
                return this.variables;
            }

            private set
            {
                var currentValue = this.variables;
                if (currentValue != value)
                {
                    this.variables = value;
                    //  this.OnPropertyChanged(nameof(this.Variables));
                }
            }
        }

        public void Run(IEnumerable<CommandData> commands)
        {
            commands = commands ?? Enumerable.Empty<CommandData>();

            // process file elements
            foreach (var command in commands)
            {
                // find the first plugin with the matching name
                var plugin = this.CommandPlugins.FirstOrDefault(c => c.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase));
                if (plugin == null)
                {
                    continue;
                }

                // setup executing args
                var executeCommandArgs = new RunCommandArgs(this.Variables, command);
                try
                {
                    // check if the command has an attached message and if so output the message before executing
                    var message = command.GetParameter("message", string.Empty);
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        message = message.ReplaceVariableStrings(this.Variables);
                        this.status?.Report($"Message: {message}");
                    }

                    // execute the command plugin
                    plugin.Run(executeCommandArgs);
                    var result = executeCommandArgs.Result;

                    if (result != null)
                    {
                        this.status?.Report($"'{command.Name}' completed with status '{result.Status}'.");
                        if (result.Status == RunStatus.Errored)
                        {
                            this.status?.Report(result.Error.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.status?.Report($"Command {plugin.Name} threw an unexpected exception.");
                    this.status?.Report(ex.ToString());
                }
            }
        }

        // protected virtual void OnOutputMessage(string message)
        // {
        //     var handler = this.OutputMessage;
        //     if (handler != null)
        //     {
        //         handler(this, new OutputEventArgs(message));
        //     }
        // }
        //
        // protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        // {
        //     var handler = this.PropertyChanged;
        //     if (handler != null)
        //     {
        //         handler(this, new PropertyChangedEventArgs(propertyName));
        //     }
        // }
    }
}