using System;
using System.IO;
using System.Linq;
using BuildHelperTests.Mocks;
using Codefarts.BuildHelper;
using Codefarts.BuildHelperConsoleApp;
using Codefarts.DependencyInjection;
using Codefarts.IoC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildHelperTests;

[TestClass]
[TestCategory($"Console Application - {nameof(PluginManager)}")]
public class PluginManagerTests
{
    private string pluginPath;

    [TestInitialize]
    public void Setup()
    {
        // config setup
        this.pluginPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var info = new DirectoryInfo(this.pluginPath);
        info.Create();

        TestHelpers.CopyDirectory(@"P:\Code Projects\Codefarts.BuildHelper\Codefarts.BuildHelperConsoleApp\bin\Debug\net6.0", this.pluginPath, true);
    }

    [TestCleanup]
    public void Cleanup()
    {
        GC.Collect();
        var info = new DirectoryInfo(this.pluginPath);
        try
        {
            info.Delete(true);
        }
        catch
        {
        }
    }

    [TestMethod]
    public void PluginLoader_Ctor_NullArguments()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            var loader = new PluginManager(null);
        });
    }

    [TestMethod]
    public void PluginLoader_Ctor_ValidArguments()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var loader = new PluginManager(ioc);
        Assert.IsNotNull(loader);
    }

    [TestMethod]
    public void PluginLoader_NoConfigProvider_Succesful()
    {
        var ioc = new DependencyInjectorShim(new Container());

        // registration
        ioc.Register<IDependencyInjectionProvider>(() => ioc);
        ioc.Register<IPluginManager, PluginManager>();
        ioc.Register<IStatusReporter, MockStatusReporter>();

        // get plugin manager
        var loader = ioc.Resolve<IPluginManager>();
        Assert.IsNotNull(loader);
    }

    [TestMethod]
    public void PluginLoader_NoPluginFolderSpecified()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var config = new MockConfigProvider();

        // registration
        ioc.Register<IDependencyInjectionProvider>(() => ioc);
        ioc.Register<IPluginManager, PluginManager>();
        ioc.Register<IConfigurationProvider>(() => config);
        ioc.Register<IStatusReporter, MockStatusReporter>();

        // get plugin manager
        var loader = ioc.Resolve<IPluginManager>();

        // run tests
        Assert.IsNotNull(loader.Plugins);
        var plugins = loader.Plugins.ToArray();
        Assert.IsNotNull(plugins);
        Assert.AreEqual(0, plugins.Length);
    }

    [TestMethod]
    public void PluginLoader_ValidPluginFolder_WithPlugins()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var config = new MockConfigProvider();
        config.SetValue("pluginfolder", this.pluginPath);

        // registration
        ioc.Register<IDependencyInjectionProvider>(() => ioc);
        ioc.Register<IPluginManager, PluginManager>();
        ioc.Register<IConfigurationProvider>(() => config);
        ioc.Register<IStatusReporter, MockStatusReporter>();

        // get plugin manager
        var loader = ioc.Resolve<IPluginManager>();

        // run tests
        Assert.IsNotNull(loader.Plugins);
        var plugins = loader.Plugins.ToArray();
        Assert.IsNotNull(plugins);
        Assert.IsTrue(plugins.Length > 0);
    }
}