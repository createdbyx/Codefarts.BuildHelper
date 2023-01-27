using System;
using System.IO;
using BuildHelperTests.Mocks;
using Codefarts.BuildHelper;
using Codefarts.BuildHelperConsoleApp;
using Codefarts.IoC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Codefarts.DependencyInjection;

namespace BuildHelperTests;

[TestClass, TestCategory("Build Command")]
public class BuildCommandTests
{
    private VariablesDictionary variables;
    private string tempDir;

    private string projectFile;
    private string buildFile;
    private string badlyNamedBuildFile;
    private string pluginPath;

    [TestInitialize]
    public void Setup()
    {
        this.pluginPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var info = new DirectoryInfo(this.pluginPath);
        info.Create();

        TestHelpers.CopyDirectory(@"P:\Code Projects\Codefarts.BuildHelper\Codefarts.BuildHelperConsoleApp\bin\Debug\net6.0", this.pluginPath, true);


        this.projectFile = Path.Combine(Directory.GetCurrentDirectory(), "SampleData", "SampleProject.csproj");
        this.buildFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "Debug-PostBuild.xml");
        this.badlyNamedBuildFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "RandomBuild.xml");

        var directoryName = Path.GetDirectoryName(this.buildFile);
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        var buildFileData = """
            <?xml version="1.0" encoding="utf-8" ?>
            <build>
                
            </build>
            """;

        File.WriteAllText(this.buildFile, buildFileData);
    }

    [TestCleanup]
    public void Cleanup()
    {
        var directoryName = Path.GetDirectoryName(this.buildFile);
        try
        {
            Directory.Delete(directoryName, true);
        }
        catch
        {
        }

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
    public void BadCommandName()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var command = new BuildCommand.BuildCommand(ioc);
        var args = new RunCommandArgs(new CommandData("badname"));
        command.Run(args);
        Assert.IsNotNull(args.Result);
        Assert.IsNotNull(args.Result.Error);
        Assert.IsInstanceOfType<ArgumentException>(args.Result.Error);
    }

    [TestMethod]
    public void RunWithValidArguments_NoConfigManager()
    {
        var ioc = new DependencyInjectorShim(new Container());

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
        ioc.Register<IConfigurationProvider, MockConfigProvider>();

        var command = new BuildCommand.BuildCommand(ioc);
        var args = new RunCommandArgs(new CommandData("build"));
        command.Run(args);
        Assert.IsNotNull(args.Result);
        Assert.IsNotNull(args.Result.Error);
        Assert.IsInstanceOfType<ArgumentNullException>(args.Result.Error);
    }

    [TestMethod]
    public void RunWithValidArguments_ConfigManagerWithBadlyNamedBuildFile()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var config = new MockConfigProvider();
        config.SetValue("filename", this.badlyNamedBuildFile);
        config.SetValue("projectfile", this.projectFile);
        config.SetValue("targetframework", "netstandard2.0");
        ioc.Register<IConfigurationProvider>(() => config);

        var command = new BuildCommand.BuildCommand(ioc);
        var args = new RunCommandArgs(new CommandData("build"));
        command.Run(args);
        Assert.IsNotNull(args.Result);
        Assert.IsNotNull(args.Result.Error);
        Assert.AreEqual(typeof(Exception).FullName, args.Result.Error.GetType().FullName);
    }

    [TestMethod]
    public void RunWithValidArguments_ConfigManagerWithValidData_NoPluginManagerRegisterd()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var config = new MockConfigProvider();
        config.SetValue("filename", this.buildFile);
        config.SetValue("projectfile", this.projectFile);
        config.SetValue("targetframework", "netstandard2.0");
        ioc.Register<IConfigurationProvider>(() => config);

        var command = new BuildCommand.BuildCommand(ioc);
        var args = new RunCommandArgs(new CommandData("build"));
        command.Run(args);
        Assert.IsNotNull(args.Result);
        Assert.IsNotNull(args.Result.Error);
        Assert.IsInstanceOfType<ContainerResolutionException>(args.Result.Error);
    }

    [TestMethod]
    public void RunWithValidArguments_ConfigManagerWithValidData_ValidPluginManagerRegisterd()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var config = new MockConfigProvider();
        config.SetValue("filename", this.buildFile);
        config.SetValue("projectfile", this.projectFile);
        config.SetValue("targetframework", "netstandard2.0");
        config.SetValue("pluginfolder", this.pluginPath);

        ioc.Register<IDependencyInjectionProvider>(() => ioc);
        ioc.Register<IPluginManager, PluginManager>();
        ioc.Register<IConfigurationProvider>(() => config);

        var command = new BuildCommand.BuildCommand(ioc);
        var args = new RunCommandArgs(new CommandData("build"));
        command.Run(args);
        Assert.IsNotNull(args.Result);
        Assert.IsNull(args.Result.Error);
    }
}