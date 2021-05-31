// <copyright file="RemoveVariableCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System.Xml;

    [NamedParameter("name", typeof(string), true, "The name of the variable.")]
    public class RemoveVariableCommand : ICommandPlugin
    {
        public string Name => "removevariable";

        public void Run(RunCommandArgs args)
        {
            // get variable name
            var nameValue = args.GetParameter<string>("name");
            if (nameValue == null)
            {
                throw new XmlException($"Command: {nameof(RemoveVariableCommand)} value: name - Value not found");
            }

            // replace variable strings
            nameValue = nameValue.ReplaceVariableStrings(args.Variables);

            // remove the variable and report result
            if (args.Variables.Remove(nameValue))
            {
                args.Output($"Variable Removed: {nameValue}");
            }
            else
            {
                args.Output($"Variable NOT Removed: {nameValue}");
            }
        }
    }
}