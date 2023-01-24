namespace Codefarts.BuildHelper;

public static class IConfigurationProviderExtensionMethods
{
    public static T GetValue<T>(this IConfigurationProvider config, string name, T defaultValue)
    {
        if (TryGetValue<T>(config, name, out var value))
        {
            return value;
        }

        return defaultValue;
    }

    public static T GetValue<T>(this IConfigurationProvider config, string name)
    {
        return (T)config.GetValue(name);
    }

    public static bool TryGetValue<T>(this IConfigurationProvider config, string name, out T value)
    {
        try
        {
            value = (T)config.GetValue(name);
            return true;
        }
        catch
        {
            value = default;
            return false;
        }
    }
}