using System.Collections.Generic;
using Codefarts.BuildHelper;

namespace BuildHelperTests.Mocks;

public class MockConfigManager : IConfigurationManager
{
    public MockConfigManager()
    {
        this.Values = new Dictionary<string, object>();
    }

    public MockConfigManager(IDictionary<string, object> values)
    {
        this.Values = values;
    }

    public IDictionary<string, object> Values { get; set; }

    public object GetValue(string name)
    {
        return this.Values[name];
    }
}