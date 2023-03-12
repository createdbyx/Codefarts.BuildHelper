using System;
using System.IO;
using Codefarts.BuildHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildHelperTests;

[TestClass, TestCategory(nameof(PowerShellCommand.PowerShellCommand))]
public class PowerShellTests
{
    private VariablesDictionary variables;
    private ICommandPlugin plugin;

    [TestInitialize]
    public void InitTest()
    {
        this.plugin = new PowerShellCommand.PowerShellCommand();
        this.variables = new VariablesDictionary();
        this.variables["BuildFile"] = Path.Combine(Environment.CurrentDirectory, "MissingFile.xml");
    }

    [TestCleanup]
    public void TestCleanup()
    {
        this.plugin = null;
        this.variables = null;
    }

    [TestMethod]
    public void ValidateCommandName()
    {
        Assert.AreEqual("powershell", this.plugin.Name);
    }

    [TestMethod]
    public void RunWithNullArgs()
    {
        Assert.ThrowsException<ArgumentNullException>(() => this.plugin.Run(null));
    }

    [TestMethod]
    public void ValidArgsButNoScriptOrFileParam()
    {
        var command = new CommandData("powershell");
        var args = new RunCommandArgs(this.variables, command);
        this.plugin.Run(args);
        Assert.AreEqual(RunStatus.Sucessful, args.Result.Status);
    }

    [TestMethod]
    public void SimpleEchoScript()
    {
        var command = new CommandData("powershell");
        command.Parameters["Value"] = "echo Test";
        var args = new RunCommandArgs(this.variables, command);
        this.plugin.Run(args);
        Assert.AreEqual(RunStatus.Sucessful, args.Result.Status);
    }


    [TestMethod]
    public void MissingScriptFileAndNoEmbeded()
    {
        var command = new CommandData("powershell");
        command.Parameters["File"] = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".ps1");
        var args = new RunCommandArgs(this.variables, command);
        this.plugin.Run(args);
        Assert.AreEqual(RunStatus.Sucessful, args.Result.Status);
    }
}