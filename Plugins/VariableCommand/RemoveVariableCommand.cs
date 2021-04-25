// <copyright file="RemoveVariableCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System.Xml;

    public class RemoveVariableCommand : IBuildCommand
    {
        public string Name => "removevariable";

        public void Execute(ExecuteCommandArgs args)
        {
            var nameValue = args.GetParameter<string>("name");
            if (nameValue == null)
            {
                throw new XmlException($"Command: {nameof(RemoveVariableCommand)} value: name - Value not found");
            }

            nameValue = nameValue.ReplaceBuildVariableStrings(args.Variables);

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