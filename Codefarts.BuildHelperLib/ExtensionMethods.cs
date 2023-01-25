// <copyright file="ExtensionMethods.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System.Runtime.InteropServices;

namespace Codefarts.BuildHelper;

using System;

public static class ExtensionMethods
{
    public static bool SatifiesCondition(string value1, string value2, string operatorValue, bool ignoreCase)
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

            case "missing":
            case "ismissing":
            case "notfound":
            case "!contains":
                if (ignoreCase ? value1.IndexOf(value2, StringComparison.OrdinalIgnoreCase) >= 0 : value1.Contains(value2))
                {
                    return false;
                }

                return true;

            case null:
                throw new ArgumentNullException("Condition is missing 'operator' value.");

            default:
                throw new ArgumentOutOfRangeException("'operator' attribute exists but it's meaning could not determined.");
        }
    }
}