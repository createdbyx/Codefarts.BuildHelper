using System;
using BuildHelperTests.Mocks;
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
    public void CtorValidArgs_WithRegisteredStatusReporter()
    {
        var ioc = new DependencyInjectorShim(new Container());
        ioc.Register(typeof(IStatusReporter), typeof(MockStatusReporter));
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
        ioc.Register(typeof(ICommandImporter), () => new XmlCommandFileReader(ioc));

        // var importer = ioc.Resolve(typeof(ICommandImporter)) as ICommandImporter;
        // var importResult = importer.Run();

        var command = new BuildCommand.BuildCommand(ioc);
        var args = new RunCommandArgs(new CommandData("build"));
        command.Run(args);
        Assert.IsNotNull(args.Result);
        Assert.IsNotNull(args.Result.Error);
        Assert.IsInstanceOfType<ContainerResolutionException>(args.Result.Error);
    }


    [TestMethod]
    public void RunWithValidArguments_ConfigManagerWithNoData()
    {
        var ioc = new DependencyInjectorShim(new Container());
        ioc.Register(typeof(ICommandImporter), () => new XmlCommandFileReader(ioc));
        ioc.Register(typeof(IConfigurationProvider), typeof(MockConfigProvider));

        var command = new BuildCommand.BuildCommand(ioc);
        var args = new RunCommandArgs(new CommandData("build"));
        command.Run(args);
        Assert.IsNotNull(args.Result);
        Assert.IsNotNull(args.Result.Error);
        Assert.IsInstanceOfType<ArgumentNullException>(args.Result.Error);
    }
}