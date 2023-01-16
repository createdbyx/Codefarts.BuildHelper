using System;
using System.Dynamic;
using Codefarts.BuildHelper;
using Codefarts.BuildHelperConsoleApp;
using Codefarts.IoC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildHelperTests;

[TestClass, TestCategory("Build Command")]
public class BuildCommandTests
{
    private VariablesDictionary variables;
    private string tempDir;

    [TestInitialize]
    public void InitTest()
    {
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }

    [TestMethod]
    public void CtorNullArgs()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new BuildCommand.BuildCommand(null));
    }

    [TestMethod]
    public void CtorValidArgs()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var b = new BuildCommand.BuildCommand(ioc);
        
    }
    
    [TestMethod]
    public void ValidateCommandName()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var b = new BuildCommand.BuildCommand(ioc);
        Assert.AreEqual("build", b.Name);
    }


    [TestMethod]
    public void RunWithNullArguments()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var b = new BuildCommand.BuildCommand(ioc);
        Assert.ThrowsException<ArgumentNullException>(() => b.Run(null));
    }
    
    [TestMethod]
    public void RunWithValidArguments_NoConfigManager()
    {
        var ioc = new DependencyInjectorShim(new Container());
        ioc.Register(typeof(ICommandImporter) ,()=> new XmlCommandFileReader(ioc));

        var importer = ioc.Resolve(typeof(ICommandImporter)) as ICommandImporter;
        var importResult = importer.Run();
        
        var b = new BuildCommand.BuildCommand(ioc);
        var args=new RunCommandArgs(new CommandData());
        Assert.ThrowsException<ArgumentNullException>(() => b.Run(args));
    }
}