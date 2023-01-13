using System;
using System.Diagnostics;
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
//        var tempPath = Path.Combine( Path.GetTempPath(), nameof(XmlCommandFileReaderTests), "_", Guid.NewGuid().ToString("N"), "Build_DeployWithOneCondition.xml");
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
    }
}