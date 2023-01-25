using System;
using System.Globalization;
using Codefarts.BuildHelper.Exceptions;

namespace Codefarts.BuildHelper;

public static class RunCommandArgsExtensionMethods
{
    public static T GetParameter<T>(this RunCommandArgs args, string name)
    {
        if (args == null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        object value;
        if (!args.Command.Parameters.TryGetValue(name, out value))
        {
            throw new MissingParameterException(name);
        }

        if (value is IConvertible)
        {
            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }

        return (T)value;
    }

    public static T GetParameter<T>(this RunCommandArgs args, string name, T defaultValue)
    {
        if (args == null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        return args.Command.Parameters.GetValue<T>(name, defaultValue);
    }

    public static T GetVariable<T>(this RunCommandArgs args, string name)
    {
        if (args == null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        object value;
        if (!args.Variables.TryGetValue(name, out value))
        {
            throw new MissingVariableException(name);
        }

        if (value is IConvertible)
        {
            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }

        return (T)value;
    }

    public static T GetVariable<T>(this RunCommandArgs args, string name, T defaultValue)
    {
        if (args == null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        object value;
        if (args.Variables.TryGetValue(name, out value))
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