// <copyright file="ExtensionMethods.cs" company="Codefarts">
// Copyright (c) Codefarts
// </copyright>

using System;

namespace Codefarts.BuildHelper
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    public static class Extensions
    {
        public static string GetValue(this XElement element, string name)
        {
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
                return null;
            }

            foreach (var item in variables)
            {
                text = text.Replace($"$({item.Key})", item.Value);
            }

            return text;
        }

        public static bool SatifiesConditions(this XElement element, IDictionary<string, string> variables)
        {
            var conditions = element.Elements().Where(x => x.Name.LocalName == "condition");
            foreach (var condition in conditions)
            {
                var value1 = condition.GetValue("value1").ReplaceBuildVariableStrings(variables);
                var value2 = condition.GetValue("value2").ReplaceBuildVariableStrings(variables);
                var op = condition.GetValue("operator");
                var ignoreCaseValue = condition.GetValue("ignorecase");
                bool ignoreCase;
                bool.TryParse(ignoreCaseValue, out ignoreCase);

                switch (op)
                {
                    case "=":
                    case "equals":
                        if (ignoreCase ? string.Equals(value1, value2, StringComparison.OrdinalIgnoreCase) : string.Equals(value1, value2))
                        {
                            continue;
                        }

                        return false;

                    case "!=":
                    case "notequal":
                        if (ignoreCase ? !string.Equals(value1, value2, StringComparison.OrdinalIgnoreCase) : !string.Equals(value1, value2))
                        {
                            continue;
                        }

                        return false;
                }
            }

            return true;
        }
    }
}