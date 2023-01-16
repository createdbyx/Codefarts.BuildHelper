using System;
using System.IO;
using Codefarts.BuildHelperConsoleApp;
using Codefarts.DependencyInjection;
using Codefarts.IoC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildHelperTests;

[TestClass]
[TestCategory("Console Application - PluginLoader")]
public class PluginLoaderTests
{
    [TestMethod]
    public void PluginLoader_Ctor_NullArguments()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            var loader = new PluginLoader(null);
        });
    }

    [TestMethod]
    public void PluginLoader_Ctor_ValidArguments()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var loader = new PluginLoader(ioc);
        Assert.IsNotNull(loader);
    }

    [TestMethod]
    public void PluginLoader_MissingPluginFolder()
    {
        var ioc = new DependencyInjectorShim(new Container());
        ioc.Register<IDependencyInjectionProvider>(() => ioc);
        var loader = new PluginLoader(ioc);
        var pluginPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        loader.PluginFolder = pluginPath;
        var plugins = loader.Load();
        Assert.IsNotNull(plugins);
        Assert.AreEqual(0, plugins.Count);
    }
}