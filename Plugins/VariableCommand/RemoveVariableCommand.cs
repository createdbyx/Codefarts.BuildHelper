// <copyright file="RemoveVariableCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;

    [NamedParameter("name", typeof(string), true, "The name of the variable.")]
    public class RemoveVariableCommand : ICommandPlugin
    {
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
            var wasRemoved = args.Variables.Remove(nameValue);
            if (wasRemoved)
            {
                args.Output($"Variable Removed: {nameValue}");
            }
            else
            {
                args.Output($"Variable NOT Removed: {nameValue}");
            }

            args.Result = RunResult.Sucessful(wasRemoved);
        }
    }
}