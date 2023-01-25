using System.Collections.Generic;

namespace Codefarts.BuildHelper;

public static class StringExtensionMethods
{
    public static string ReplaceVariableStrings(this string text, IDictionary<string, object> variables)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return text;
        }

        foreach (var item in variables)
        {
            if (item.Value != null)
            {
                text = text.Replace($"$({item.Key})", item.Value.ToString());
            }
        }

        return text;
    }
}