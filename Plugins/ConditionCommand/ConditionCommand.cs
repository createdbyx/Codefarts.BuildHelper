// <copyright file="ConditionCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace ConditionCommand
{
    using System;
    using Codefarts.BuildHelper;

    [NamedParameter("value1", typeof(object), true, "The first value to be compared.")]
    [NamedParameter("value2", typeof(object), true, "The second value to be compared.")]
    [NamedParameter("operator", typeof(string), true, "The operator that defines how values will be compared.")]
    [NamedParameter("ignorecase", typeof(bool), false, "Ignores casing if comparing strings.")]
    public class ConditionCommand : ICommandPlugin
    {
        public string Name
        {
            get
            {
                return "condition";
            }
        }

        public void Run(RunCommandArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            // get values
            var value1 = args.GetParameter<object>("value1", null);
            var value2 = args.GetParameter<object>("value2", null);
            var operatorValue = args.GetParameter("operator", string.Empty);
            var ignoreCase = args.GetParameter("ignorecase", false);

            // perform validation
            if (value1 == null)
            {
                args.Result = RunResult.Errored(new MissingParameterException(nameof(value1)));
                return;
            }

            if (value2 == null)
            {
                args.Result = RunResult.Errored(new MissingParameterException(nameof(value2)));
                return;
            }

            if (string.IsNullOrWhiteSpace(operatorValue))
            {
                args.Result = RunResult.Errored(new MissingParameterException("operator"));
                return;
            }

            // get types
            var value1Type = value1.GetType();
            var value2Type = value2.GetType();

            // compare value types
            if ((value1Type.IsValueType | value1Type == typeof(string)) && (value2Type.IsValueType | value2Type == typeof(string)))
            {
                var string1 = value1.ToString().ReplaceVariableStrings(args.Variables);
                var string2 = value2.ToString().ReplaceVariableStrings(args.Variables);
                try
                {
                    var result = this.CompareStrings(string1, string2, operatorValue, ignoreCase);
                    args.Result = RunResult.Sucessful(result);
                    return;
                }
                catch (Exception ex)
                {
                    args.Result = RunResult.Errored(ex);
                    return;
                }
            }

            // currently only support value type comparisons including strings
            args.Result = RunResult.Errored(new NotSupportedException("Only value type comparisons and strings are supported."));
        }

        private bool CompareStrings(string value1, string value2, string operatorValue, bool ignoreCase)
        {
            operatorValue = operatorValue.ToLowerInvariant().Trim();
            var comparisonType = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;
            switch (operatorValue)
            {
                case "=":
                case "equals":
                case "equalto":
                    return string.Equals(value1, value2, comparisonType);

                case "!=":
                case "notequal":
                case "notequalto":
                    return !string.Equals(value1, value2, comparisonType);

                case "startswith":
                case "beginswith":
                    return value1.StartsWith(value2, comparisonType);

                case "endswith":
                    return value1.EndsWith(value2, comparisonType);

                case "contains":
                    return value1.Contains(value2, comparisonType);

                default:
                    throw new NotSupportedException($"'operator' value not supported. Value was '{operatorValue}'.");
            }
        }
    }
}