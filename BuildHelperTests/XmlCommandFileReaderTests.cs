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
[TestCategory("XmlCommandFileReader")]
public class XmlCommandFileReaderTests
{
    private string configFile;

    [TestInitialize]
    public void Setup()
    {
        this.configFile = Path.Combine(Directory.GetCurrentDirectory(), "SampleData", "Build_DeployWithOneCondition.xml");
    }

    [TestCleanup]
    public void Cleanup()
    {
    }

    [TestMethod]
    public void Ctor_NullArguments()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            var xml = new XmlCommandFileReader(null);
        });
    }

    [TestMethod]
    public void Ctor_ValidArguments()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var xml = new XmlCommandFileReader(ioc);
        Assert.IsNotNull(xml);
    }

    [TestMethod]
    public void Ctor_ValidArguments_NoRegisteredConfig()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var xml = new XmlCommandFileReader(ioc);
        var result = xml.Run();
        Assert.IsNotNull(xml);
        Assert.IsNotNull(result.Error);
        Assert.IsInstanceOfType(result.Error, typeof(NullReferenceException), result.Error.Message);
    }

    [TestMethod]
    public void Ctor_ValidArguments_WithRegisteredConfig_ButNoData()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var xml = new XmlCommandFileReader(ioc);
        var config = ioc.Resolve<MockConfigProvider>();
        ioc.Register<IConfigurationProvider>(() => config);

        var result = xml.Run();
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Error);
        Assert.IsNull(result.ReturnValue);
        Assert.IsInstanceOfType<IOException>(result.Error);
    }

    [TestMethod]
    public void Ctor_ValidArguments_WithRegisteredConfig_MissingFile()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var xml = new XmlCommandFileReader(ioc);
        var config = ioc.Resolve<MockConfigProvider>();
        config.SetValue("filename", Path.Combine(Path.GetTempPath(), $"SomeRandomMissingFile_{Guid.NewGuid().ToString("N")}.xml"));
        ioc.Register<IConfigurationProvider>(() => config);

        var result = xml.Run();
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Error);
        Assert.IsNull(result.ReturnValue);
        Assert.IsInstanceOfType<IOException>(result.Error);
    }

    [TestMethod]
    public void Ctor_ValidArguments_WithRegisteredConfig_WithValidFileData()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var xml = new XmlCommandFileReader(ioc);
        var config = ioc.Resolve<MockConfigProvider>();
        config.Values["filename"] = this.configFile;
        ioc.Register<IConfigurationProvider>(() => config);

        var result = xml.Run();
        Assert.IsNotNull(result);
        Assert.IsNull(result.Error);
        Assert.IsNotNull(result.ReturnValue);
        Assert.IsInstanceOfType<CommandData>(result.ReturnValue);
    }
}