using System;
using System.Collections.Generic;
using Codefarts.BuildHelper;

namespace Codefarts.BuildHelperConsoleApp;

internal class ConfigManager : IConfigurationManager
{
    public ConfigManager(IDictionary<string, object> values)
    {
        this.Values = values ?? throw new ArgumentNullException(nameof(values));
    }

    public IDictionary<string, object> Values { get; }

    public object GetValue(string name)
    {
        return this.Values[name];
    }
}