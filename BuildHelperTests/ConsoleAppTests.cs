using System;
using System.IO;
using BuildHelperTests.Mocks;
using Codefarts.BuildHelper;
using Codefarts.BuildHelperConsoleApp;
using Codefarts.DependencyInjection;
using Codefarts.IoC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildHelperTests;

[TestClass]
[TestCategory("Console Application")]
public class ConsoleAppTests
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
    public void Ctor_NullArguments()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            var app = new Application(null);
        });
    }

    [TestMethod]
    public void Ctor_ValidArguments()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var app = new Application(ioc);
        Assert.IsNotNull(app);
    }

    [TestMethod]
    public void Run_ValidArguments_NoImporter()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var app = new Application(ioc);
        Assert.ThrowsException<ContainerResolutionException>(() => app.Run());
    }

    [TestMethod]
    public void Run_ValidArguments_WithImporter_Succesful()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var config = new MockConfigProvider();
        config.SetValue("pluginfolder", this.pluginPath);

        ioc.Register<IDependencyInjectionProvider>(() => ioc);
        ioc.Register<ICommandImporter, MockSuccsesfulCommandImporter>();
        ioc.Register<IPluginManager, PluginManager>();
        ioc.Register<IConfigurationProvider>(() => config);

        var app = ioc.Resolve<Application>();

        var result = app.Run();
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Error);
        Assert.IsNull(result.ReturnValue);
    }
}