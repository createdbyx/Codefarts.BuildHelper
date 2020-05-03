// <copyright file="ExtensionMethods.cs" company="Codefarts">
// Copyright (c) Codefarts
// </copyright>

using System.Linq;

namespace Codefarts.BuildHelper
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    public static class Extensions
    {
        public static string GetValue(this XElement element, string name)
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

        public static string ReplaceBuildVariableStrings(this string text, IDictionary<string, string> variables)
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

        public static bool SatifiesConditions(this XElement element, IDictionary<string, string> variables)
        {
            return element.SatifiesConditions(variables, true);
        }

        public static bool SatifiesConditions(this XElement element, IDictionary<string, string> variables, bool allConditions)
        {
            if (allConditions)
            {
                return element.Elements().Where(condition => condition.Name.LocalName == "condition")
                    .All(condition => condition.SatifiesCondition(variables));
            }

            return element.Elements().Where(condition => condition.Name.LocalName == "condition")
                .Any(condition => condition.SatifiesCondition(variables));
        }

        public static bool SatifiesCondition(this XElement condition, IDictionary<string, string> variables)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            if (condition.Name.LocalName != "condition")
            {
                throw new ArgumentException("Condition element name is not 'condition'.", nameof(condition));
            }

            var value1 = condition.GetValue("value1").ReplaceBuildVariableStrings(variables);
            var value2 = condition.GetValue("value2").ReplaceBuildVariableStrings(variables);
            var operatorValue = condition.GetValue("operator");
            var ignoreCaseValue = condition.GetValue("ignorecase");
            var ignoreCase = true;
            if (ignoreCaseValue != null && !bool.TryParse(ignoreCaseValue, out ignoreCase))
            {
                throw new ArgumentOutOfRangeException("'ignorecase' attribute exists but it's value could not be parsed as a bool value.");
            }

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

                case "contains":       //this.IndexOf(value, comparisonType) >= 0
                    //if (ignoreCase ? !value1.Contains(value2, StringComparison.OrdinalIgnoreCase) >= 0 : !value1.Contains(value2))
                    if (ignoreCase ? value1.IndexOf(value2, StringComparison.OrdinalIgnoreCase) >= 0 : value1.Contains(value2))
                    {
                        return true;
                    }

                    return false;

                case null:
                    throw new ArgumentNullException("Condition is missing 'operator' attribute.");

                default:
                    throw new ArgumentOutOfRangeException("'operator' attribute exists but it's meaning could not determined.");
            }

            return true;
        }
    }
}