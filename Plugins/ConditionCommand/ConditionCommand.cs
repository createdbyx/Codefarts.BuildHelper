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
    [NamedParameter("value2", typeof(object), true, "THe second value to be compared.")]
    [NamedVariable("operator", typeof(string), true, "The operator that defines how values will be compared.")]
    [NamedVariable("ignorecase", typeof(bool), false, "Ignores casing if comparing strings.")]
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
                throw new MissingParameterException(nameof(value1));
            }

            if (value2 == null)
            {
                throw new MissingParameterException(nameof(value2));
            }

            if (string.IsNullOrWhiteSpace(operatorValue))
            {
                throw new MissingParameterException("operator");
            }

            // get types
            var value1Type = value1.GetType();
            var value2Type = value2.GetType();

            // compare value types
            if ((value1Type.IsValueType | value1Type == typeof(string)) && (value2Type.IsValueType | value2Type == typeof(string)))
            {
                var string1 = value1.ToString();
                var string2 = value2.ToString();
                var result = this.CompareStrings(string1, string2, operatorValue, ignoreCase);
            }

            // compare dissimilar object types

            // ... determine what to do here
            
        }

        private bool CompareStrings(string value1, string value2, string operatorValue, bool ignoreCase)
        {
            if (value1 == null)
            {
                throw new ArgumentOutOfRangeException("'value1' is missing.", nameof(value1));
            }

            if (value2 == null)
            {
                throw new ArgumentOutOfRangeException("'value2' is missing.", nameof(value2));
            }

            operatorValue = operatorValue == null ? operatorValue : operatorValue.ToLowerInvariant().Trim();
            switch (operatorValue)
            {
                case "=":
                case "equals":
                case "equalto":
                    if (ignoreCase ? string.Equals(value1, value2, StringComparison.OrdinalIgnoreCase) : string.Equals(value1, value2))
                    {
                        return true;
                    }

                    return false;

                case "!=":
                case "notequal":
                case "notequalto":
                    if (ignoreCase ? !string.Equals(value1, value2, StringComparison.OrdinalIgnoreCase) : !string.Equals(value1, value2))
                    {
                        return true;
                    }

                    return false;

                case "startswith":
                case "beginswith":
                    if (ignoreCase ? value1.StartsWith(value2, StringComparison.OrdinalIgnoreCase) : value1.StartsWith(value2))
                    {
                        return true;
                    }

                    return false;

                case "endswith":
                    if (ignoreCase ? value1.EndsWith(value2, StringComparison.OrdinalIgnoreCase) : value1.EndsWith(value2))
                    {
                        return true;
                    }

                    return false;

                case "contains":
                    if (ignoreCase ? value1.IndexOf(value2, StringComparison.OrdinalIgnoreCase) >= 0 : value1.Contains(value2))
                    {
                        return true;
                    }

                    return false;

                case null:
                    throw new ArgumentNullException("Condition is missing 'operator' value.");

                default:
                    throw new ArgumentOutOfRangeException("'operator' attribute exists but it's meaning could not determined.");
            }
        }
    }
}