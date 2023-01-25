using System;
using System.Globalization;

namespace Codefarts.BuildHelper;

public static class RunResultExtensionMethods
{
    public static T GetReturnValue<T>(this RunResult result)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        var value = result.ReturnValue;
        if (value is IConvertible)
        {
            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }

        return (T)value;
    }

    public static T GetReturnValue<T>(this RunResult result, T defaultValue)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        try
        {
            var value = result.ReturnValue;
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
}