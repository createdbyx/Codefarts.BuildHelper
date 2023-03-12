using System.Collections.Concurrent;
using System.Xml.Linq;
using Codefarts.DependencyInjection;

namespace Codefarts.BuildHelper.XMLFileConfigManager;

public class XmlFileConfigProvider : IConfigurationProvider
{
    private IStatusReporter status;
    private readonly IDependencyInjectionProvider ioc;
    private IDictionary<string, object> values;

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlCommandFileReader"/> class.
    /// </summary>
    /// <param name="ioc">A reference to a <see cref="IDependencyInjectionProvider"/>.</param>
    /// <exception cref="ArgumentNullException">If the <param name="ioc"/> is null.</exception>
    public XmlFileConfigProvider(IDependencyInjectionProvider ioc)
    {
        this.ioc = ioc ?? throw new ArgumentNullException(nameof(ioc));
        try
        {
            this.status = ioc.Resolve<IStatusReporter>();
        }
        catch
        {
        }

        this.values = new ConcurrentDictionary<string, object>();
        //var config = ioc.Resolve<IConfigurationProvider>();
        // this.fileName = config.GetValue<string>("configfile");

        // DoLoad();
    }

    public object GetValue(string name)
    {
        return this.values[name];
    }

    public void Load(string fileName)
    {
        var fileInfo = new FileInfo(fileName);
        if (!fileInfo.Exists)
        {
            return;
        }

        // load file
        var doc = XDocument.Load(fileName);
        var newValues = new ConcurrentDictionary<string, object>();
        foreach (var x in doc.Root.Elements())
        {
            var type = Type.GetType(x.Attribute("Type").Value);
            newValues[x.Name.LocalName] = Convert.ChangeType(x.Value, type);
        }

        this.values = newValues;
    }

    public void SetValue(string name, object value)
    {
        if (value != null && !value.GetType().IsValueType && !(value is string))
        {
            this.status?.ReportError($"{nameof(XmlFileConfigProvider)} only supports value types.", nameof(XmlFileConfigProvider), 0);
            return;
        }

        values[name] = value;
    }

    public void DeleteValue(string name)
    {
        this.values.Remove(name);
    }

    public IEnumerable<string> Names
    {
        get
        {
            return new List<string>(this.values.Keys);
        }
    }

    public void Clear()
    {
        this.values.Clear();
    }
}