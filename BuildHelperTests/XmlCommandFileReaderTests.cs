using System;
using System.Collections.Generic;
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
        var config = ioc.Resolve<MockConfigManager>();
        ioc.Register<IConfigurationManager>(() => config);

        Assert.ThrowsException<KeyNotFoundException>(() => xml.Run());
    }

    [TestMethod]
    public void Ctor_ValidArguments_WithRegisteredConfig_WithData()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var xml = new XmlCommandFileReader(ioc);
        var config = ioc.Resolve<MockConfigManager>();
        config.Values["filename"] = this.configFile;
        ioc.Register<IConfigurationManager>(() => config);

        var result = xml.Run();
        Assert.IsNotNull(xml);
        Assert.IsNotNull(result.Error);
        Assert.IsNotNull(result.ReturnValue);
    }
}