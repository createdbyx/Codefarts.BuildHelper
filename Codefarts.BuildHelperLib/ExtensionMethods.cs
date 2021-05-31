// <copyright file="ExtensionMethods.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;

    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the value of a <see cref="XElement"/>.
        /// </summary>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>Value of the <see cref="XElement"/>, otherwise null.</returns>
        /// <remarks>Will return null if no value found.</remarks>
        public static string GetAttributeValue(this XElement element, string name)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            var attr = element.Attribute(name);
            if (attr != null)
            {
                return attr.Value;
            }

            var ele = element.Element(name);
            return ele != null ? ele.Value : null;
        }

        public static T GetParameter<T>(this IDictionary<string, object> parameters, string name)
        {
            return GetParameter<T>(parameters, name, default);
        }

        public static T GetParameter<T>(this CommandData commandData, string name)
        {
            return commandData.Parameters.GetParameter<T>(name, default);
        }

        public static T GetParameter<T>(this IDictionary<string, object> parameters, string name, T defaultValue)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            object value;
            if (parameters.TryGetValue(name, out value))
            {
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.CurrentCulture);
            }

            return defaultValue;
        }

        public static T GetParameter<T>(this RunCommandArgs args, string name)
        {
            return args.Command.GetParameter<T>(name, default);
        }

        public static T GetParameter<T>(this RunCommandArgs args, string name, T defaultValue)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            return args.Command.GetParameter<T>(name, defaultValue);
        }

        public static T GetVariable<T>(this RunCommandArgs args, string name)
        {
            return args.GetVariable<T>(name, default);
        }

        public static T GetVariable<T>(this RunCommandArgs args, string name, T defaultValue)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            string value;
            if (args.Variables.TryGetValue(name, out value))
            {
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.CurrentCulture);
            }

            return defaultValue;
        }

        public static T GetParameter<T>(this CommandData commandData, string name, T defaultValue)
        {
            if (commandData == null)
            {
                throw new ArgumentNullException(nameof(commandData));
            }

            return commandData.Parameters.GetParameter(name, defaultValue);
        }

        public static string ReplaceVariableStrings(this string text, IDictionary<string, string> variables)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            foreach (var item in variables)
            {
                text = text.Replace($"$({item.Key})", item.Value);
            }

            return text;
        }

        public static bool SatifiesConditions(this CommandData element, IDictionary<string, string> variables)
        {
            return element.SatifiesConditions(variables, true);
        }

        public static bool SatifiesConditions(this CommandData element, IDictionary<string, string> variables, bool allConditions)
        {
            if (allConditions)
            {
                return element.Children.Where(condition => condition.Name == "condition")
                    .All(condition => condition.SatifiesCondition(variables));
            }

            return element.Children.Where(condition => condition.Name == "condition")
                .Any(condition => condition.SatifiesCondition(variables));
        }

        public static bool SatifiesCondition(this CommandData condition, IDictionary<string, string> variables)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            if (condition.Name != "condition")
            {
                throw new ArgumentException("Condition element name is not 'condition'.", nameof(condition));
            }

            var value1 = condition.GetParameter<string>("value1").ReplaceVariableStrings(variables);
            var value2 = condition.GetParameter<string>("value2").ReplaceVariableStrings(variables);
            var operatorValue = condition.GetParameter<string>("operator");
            var ignoreCaseValue = condition.GetParameter<string>("ignorecase");
            var ignoreCase = true;
            if (ignoreCaseValue != null && !bool.TryParse(ignoreCaseValue, out ignoreCase))
            {
                throw new ArgumentOutOfRangeException("'ignorecase' attribute exists but it's value could not be parsed as a bool value.");
            }

            return SatifiesCondition(variables, value1, value2, operatorValue, ignoreCase);

            //if (value1 == null)
            //{
            //    throw new ArgumentOutOfRangeException("'value1' is missing.", nameof(value1));
            //}

            //if (value2 == null)
            //{
            //    throw new ArgumentOutOfRangeException("'value2' is missing.", nameof(value2));
            //}


            //operatorValue = operatorValue == null ? operatorValue : operatorValue.ToLowerInvariant().Trim();
            //switch (operatorValue)
            //{
            //    case "=":
            //    case "equals":
            //    case "equalto":
            //        if (ignoreCase ? string.Equals(value1, value2, StringComparison.OrdinalIgnoreCase) : string.Equals(value1, value2))
            //        {
            //            return true;
            //        }

            //        return false;

            //    case "!=":
            //    case "notequal":
            //    case "notequalto":
            //        if (ignoreCase ? !string.Equals(value1, value2, StringComparison.OrdinalIgnoreCase) : !string.Equals(value1, value2))
            //        {
            //            return true;
            //        }

            //        return false;

            //    case "startswith":
            //    case "beginswith":
            //        if (ignoreCase ? value1.StartsWith(value2, StringComparison.OrdinalIgnoreCase) : value1.StartsWith(value2))
            //        {
            //            return true;
            //        }

            //        return false;

            //    case "endswith":
            //        if (ignoreCase ? value1.EndsWith(value2, StringComparison.OrdinalIgnoreCase) : value1.EndsWith(value2))
            //        {
            //            return true;
            //        }

            //        return false;

            //    case "contains":       //this.IndexOf(value, comparisonType) >= 0
            //        //if (ignoreCase ? !value1.Contains(value2, StringComparison.OrdinalIgnoreCase) >= 0 : !value1.Contains(value2))
            //        if (ignoreCase ? value1.IndexOf(value2, StringComparison.OrdinalIgnoreCase) >= 0 : value1.Contains(value2))
            //        {
            //            return true;
            //        }

            //        return false;

            //    case null:
            //        throw new ArgumentNullException("Condition is missing 'operator' attribute.");

            //    default:
            //        throw new ArgumentOutOfRangeException("'operator' attribute exists but it's meaning could not determined.");
            //}

            //return true;
        }

        public static bool SatifiesCondition(this IDictionary<string, string> variables, string value1, string value2, string operatorValue,
            bool ignoreCase)
        {
            //var value1 = condition.GetParameter<string>("value1").ReplaceBuildVariableStrings(variables);
            //var value2 = condition.GetParameter<string>("value2").ReplaceBuildVariableStrings(variables);
            //var operatorValue = condition.GetParameter<string>("operator");
            //var ignoreCaseValue = condition.GetParameter<string>("ignorecase");
            //var ignoreCase = true;
            //if (ignoreCaseValue != null && !bool.TryParse(ignoreCaseValue, out ignoreCase))
            //{
            //    throw new ArgumentOutOfRangeException("'ignorecase' attribute exists but it's value could not be parsed as a bool value.");
            //}

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

                case "contains": //this.IndexOf(value, comparisonType) >= 0
                    //if (ignoreCase ? !value1.Contains(value2, StringComparison.OrdinalIgnoreCase) >= 0 : !value1.Contains(value2))
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