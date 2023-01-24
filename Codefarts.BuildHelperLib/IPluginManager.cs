using System.Collections.Generic;

namespace Codefarts.BuildHelper;

public interface IPluginManager
{
    public IEnumerable<ICommandPlugin> Plugins { get; }
}