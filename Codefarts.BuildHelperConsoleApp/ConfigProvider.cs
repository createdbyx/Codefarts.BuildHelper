using System;
using System.Collections.Generic;
using Codefarts.BuildHelper;

namespace Codefarts.BuildHelperConsoleApp;

internal class ConfigProvider : IConfigurationProvider
{
    public ConfigProvider(IDictionary<string, object> values)
    {
        this.Values = values ?? throw new ArgumentNullException(nameof(values));
    }

    public IDictionary<string, object> Values { get; }

    public object GetValue(string name)
    {
        return Values[name];
    }

    public void SetValue(string name, object value)
    {
        Values[name] = value;
    }

    public void DeleteValue(string name)
    {
        Values.Remove(name);
    }

    public IEnumerable<string> Names { get; }
    public void Clear()
    {
        Values.Clear();
    }
}