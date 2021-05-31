// <copyright file="AssignVariableCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System.Xml;

    [NamedParameter("name", typeof(string), true, "The name of the variable.")]
    [NamedParameter("value", typeof(string), true, "The value of the variable.")]
    public class AssignVariableCommand : ICommandPlugin
    {
        public string Name => "variable";

        public void Run(RunCommandArgs args)
        {
            // get the variable name parameter
            var nameValue = args.GetParameter<string>("name");
            if (nameValue == null)
            {
                throw new XmlException($"Command: {nameof(AssignVariableCommand)} value: name - Value not found");
            }

            // get the variable value parameter
            var valueValue = args.GetParameter<string>("value");
            if (valueValue == null)
            {
                throw new XmlException($"Command: {nameof(AssignVariableCommand)} value: value - Value not found");
            }

            // replace variable strings
            nameValue = nameValue.ReplaceVariableStrings(args.Variables);
            valueValue = valueValue.ReplaceVariableStrings(args.Variables);

            // check if variable already exists
            var existing = args.Variables.ContainsKey(nameValue);
            var oldValue = existing ? args.Variables[nameValue] : null;
            args.Variables[nameValue] = valueValue;

            // report value assignment info
            var msgPart = existing ? "existing " : string.Empty;
            args.Output($"Assigned value to {msgPart}variable.");
            if (existing)
            {
                args.Output($"Name: {nameValue} Old Value: {oldValue}");
            }

            args.Output($"Name: {nameValue} Value: {valueValue}");
        }
    }
}