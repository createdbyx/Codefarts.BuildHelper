// <copyright file="RemoveVariableCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using Codefarts.BuildHelper.Exceptions;

namespace Codefarts.BuildHelper
{
    using System;

    [NamedParameter("name", typeof(string), true, "The name of the variable.")]
    public class RemoveVariableCommand : ICommandPlugin
    {
        private IStatusReporter status;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveVariableCommand"/> class.
        /// </summary>
        public RemoveVariableCommand(IStatusReporter status)
        {
            this.status = status ?? throw new ArgumentNullException(nameof(status));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveVariableCommand"/> class.
        /// </summary>
        public RemoveVariableCommand()
        {
        }

        public string Name => "removevariable";

        public void Run(RunCommandArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            // get variable name
            var nameValue = args.GetParameter<string>("name", null);
            if (nameValue == null)
            {
                args.Result = RunResult.Errored(new MissingParameterException("name"));
                return;
            }

            // replace variable strings
            nameValue = nameValue.ReplaceVariableStrings(args.Variables);

            // remove the variable and report result
            object value;
            var wasRemoved = args.Variables.TryRemove(nameValue, out value);
            if (wasRemoved)
            {
                this.status?.Report($"Variable Removed: {nameValue}");
            }
            else
            {
                this.status?.Report($"Variable NOT Removed: {nameValue}");
            }

            args.Result = RunResult.Sucessful(wasRemoved);
        }
    }
}