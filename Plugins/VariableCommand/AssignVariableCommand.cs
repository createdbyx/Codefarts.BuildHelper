// <copyright file="AssignVariableCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System.Xml;

    public class AssignVariableCommand : IBuildCommand
    {
        public string Name => "variable";

        public void Execute(ExecuteCommandArgs args)
        {
            var nameValue = args.GetParameter<string>("name");
            if (nameValue == null)
            {
                throw new XmlException($"Command: {nameof(AssignVariableCommand)} value: name - Value not found");
            }

            var valueValue = args.GetParameter<string>("value");
            if (valueValue == null)
            {
                throw new XmlException($"Command: {nameof(AssignVariableCommand)} value: value - Value not found");
            }

            nameValue = nameValue.ReplaceVariableStrings(args.Variables);
            valueValue = valueValue.ReplaceVariableStrings(args.Variables);

            var existing = args.Variables.ContainsKey(nameValue);
            var oldValue = existing ? args.Variables[nameValue] : null;
            args.Variables[nameValue] = valueValue;

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