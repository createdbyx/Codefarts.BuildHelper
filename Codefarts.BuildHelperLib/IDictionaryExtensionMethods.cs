using System;
using System.Collections.Generic;
using System.Globalization;

namespace Codefarts.BuildHelper;

public static class IDictionaryExtensionMethods
{
    public static T GetValue<T>(this IDictionary<string, object> parameters, string name)
    {
        if (parameters == null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        object value;
        if (!parameters.TryGetValue(name, out value))
        {
            throw new KeyNotFoundException(name);
        }

        if (value is IConvertible)
        {
            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }

        return (T)value;
    }

    public static T GetValue<T>(this IDictionary<string, object> parameters, string name, T defaultValue)
    {
        if (parameters == null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        object value;
        if (parameters.TryGetValue(name, out value))
        {
            try
            {
                if (value is IConvertible)
                {
                    return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
                }

                return (T)value;
            }
            catch
            {
                return defaultValue;
            }
        }

        return defaultValue;
    }
}