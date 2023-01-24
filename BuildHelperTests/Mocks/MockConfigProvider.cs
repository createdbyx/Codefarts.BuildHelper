using System.Collections.Generic;
using System.Linq;
using Codefarts.BuildHelper;

namespace BuildHelperTests.Mocks;

public class MockConfigProvider : IConfigurationProvider
{
    public MockConfigProvider()
    {
        this.Values = new Dictionary<string, object>();
    }

    public MockConfigProvider(IDictionary<string, object> values)
    {
        this.Values = values;
    }

    public IDictionary<string, object> Values { get; set; }

    public object GetValue(string name)
    {
        return this.Values[name];
    }

    public void SetValue(string name, object value)
    {
        Values[name] = value;
    }

    public void DeleteValue(string name)
    {
        Values.Remove(name);
    }

    public IEnumerable<string> Names
    {
        get
        {
            return Values.Keys.ToArray();
        }
    }

    public void Clear()
    {
        Values.Clear();
    }
}