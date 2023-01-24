using System.Collections.Generic;

namespace Codefarts.BuildHelper;

public interface IConfigurationProvider
{
    object GetValue(string name);
    void SetValue(string name, object value);
    void DeleteValue(string name);
    IEnumerable<string> Names { get; }
    void Clear();
}