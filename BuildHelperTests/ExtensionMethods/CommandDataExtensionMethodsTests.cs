using System;
using System.Xml.Linq;
using BuildHelperTests.Mocks;
using Codefarts.BuildHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildHelperTests;

[TestClass]
[TestCategory("Extension Methods - CommandData")]
public class CommandDataExtensionMethodsTests
{
    [TestMethod]
    public void RunCommandArrayWithNullArgs()
    {
        CommandData[] commands = null;
        var ex = Assert.ThrowsException<ArgumentNullException>(() => CommandDataExtensionMethods.Run(commands, null, null, null));
        Assert.AreEqual("commands", ex.ParamName);
    }

    [TestMethod]
    public void RunCommandArrayWithNullArgsButValidCommands()
    {
        var commands = new[]
        {
            new CommandData("One"),
            new CommandData("Two"),
            new CommandData("Three"),
        };

        var ex = Assert.ThrowsException<ArgumentNullException>(() => CommandDataExtensionMethods.Run(commands, null, null, null));
        Assert.AreEqual("variables", ex.ParamName);
    }

    [TestMethod]
    public void RunCommandArrayWithNullArgsButValidCommandsAndVariables()
    {
        var commands = new[]
        {
            new CommandData("One"),
            new CommandData("Two"),
            new CommandData("Three"),
        };

        var ex = Assert.ThrowsException<ArgumentNullException>(
            () => CommandDataExtensionMethods.Run(commands, new VariablesDictionary(), null, null));
        Assert.AreEqual("plugins", ex.ParamName);
    }

    [TestMethod]
    public void RunCommandArrayWithValidArgumentsNoStatus()
    {
        var commands = new[]
        {
            new CommandData("One"),
            new CommandData("Two"),
            new CommandData("Three"),
        };

        string returnValue = null;
        var variables = new VariablesDictionary();
        var plugins = new PluginCollection();
        plugins.Add(new MockCommandPlugin("One", x => returnValue = x.Command.Name));

        CommandDataExtensionMethods.Run(commands, variables, plugins, null);
        Assert.IsNotNull(returnValue);
        Assert.AreEqual("One", returnValue);
    }

    [TestMethod]
    public void RunCommandArrayWithValidArgumentsNoStatusNoPlugins()
    {
        var commands = new[]
        {
            new CommandData("One"),
            new CommandData("Two"),
            new CommandData("Three"),
        };

        string returnValue = null;
        var variables = new VariablesDictionary();
        var plugins = new PluginCollection();

        CommandDataExtensionMethods.Run(commands, variables, plugins, null);
    }

    // [TestMethod]
    // public void RunCommandWithNullArgs()
    // {
    //     CommandData command = null;
    //     var ex = Assert.ThrowsException<ArgumentNullException>(() => ExtensionMethods.Run(command, null, null, null));
    //     Assert.AreEqual("command", ex.ParamName);
    // }
    //
    // [TestMethod]
    // public void RunCommandWithNullArgsButValidCommand()
    // {
    //     var command = new CommandData("Test");
    //     var ex = Assert.ThrowsException<ArgumentNullException>(() => ExtensionMethods.Run(command, null, null, null));
    //     Assert.AreEqual("variables", ex.ParamName);
    // }
    //
    // [TestMethod]
    // public void RunCommandWithNullArgsButValidCommandAndVariables()
    // {
    //     var command = new CommandData("Test");
    //     var ex = Assert.ThrowsException<ArgumentNullException>(() => ExtensionMethods.Run(command, new VariablesDictionary(), null, null));
    //     Assert.AreEqual("plugin", ex.ParamName);
    // }

    [TestMethod]
    public void RunCommandWithValidArgsButNoStatusReporter()
    {
        var called = false;
        var command = new CommandData("Test");
        var plugin = new MockCommandPlugin("Test", x => called = true);
        CommandDataExtensionMethods.Run(command, new VariablesDictionary(), plugin, null);
        Assert.IsTrue(called);
    }

    [TestMethod]
    public void RunCommandWithValidArgsButNoStatusReporterAndThrowsException()
    {
        var called = false;
        var command = new CommandData("Test");
        var plugin = new MockCommandPlugin("Test", x =>
        {
            throw new NotImplementedException();
#pragma warning disable 162
            called = true;
#pragma warning restore 162
        });

        Assert.ThrowsException<NotImplementedException>(() => CommandDataExtensionMethods.Run(command, new VariablesDictionary(), plugin, null));
        Assert.IsFalse(called);
    }

    [TestMethod]
    public void RunCommandWithValidArgsAndStatusReporterAndThrowsException()
    {
        var called = false;
        var command = new CommandData("Test");
        var reporter = new MockStatusReporter();
        var plugin = new MockCommandPlugin("Test", x =>
        {
            throw new NotImplementedException();
#pragma warning disable 162
            called = true;
#pragma warning restore 162
        });

        Assert.ThrowsException<NotImplementedException>(
            () => CommandDataExtensionMethods.Run(command, new VariablesDictionary(), plugin, reporter));
        Assert.IsFalse(called);
        Assert.AreEqual(2, reporter.CallCount);
    }

    [TestMethod]
    public void RunCommandWithValidArgsAndStatusReporterSucsesful()
    {
        var called = false;
        var command = new CommandData("Test");
        var reporter = new MockStatusReporter();
        var plugin = new MockCommandPlugin("Test", x =>
        {
            x.Result = RunResult.Sucessful();
            called = true;
        });

        var args = CommandDataExtensionMethods.Run(command, new VariablesDictionary(), plugin, reporter);
        Assert.IsTrue(called);
        Assert.AreEqual(1, reporter.CallCount);
        Assert.IsNotNull(args.Result);
        Assert.AreEqual(RunStatus.Sucessful, args.Result.Status);
    }

    [TestMethod]
    public void RunCommandWithValidArgsAndStatusReporterErrored()
    {
        var called = false;
        var command = new CommandData("Test");
        var reporter = new MockStatusReporter();
        var plugin = new MockCommandPlugin("Test", x =>
        {
            x.Result = RunResult.Errored(new NotImplementedException());
            called = true;
        });

        var args = CommandDataExtensionMethods.Run(command, new VariablesDictionary(), plugin, reporter);
        Assert.IsTrue(called);
        Assert.AreEqual(2, reporter.CallCount);
        Assert.IsNotNull(args.Result);
        Assert.AreEqual(RunStatus.Errored, args.Result.Status);
    }


    [TestMethod]
    public void RunCommandWithValidArgsAndNoVariables()
    {
        var called = false;
        var command = new CommandData("Test");
        var reporter = new MockStatusReporter();
        var plugin = new MockCommandPlugin("Test", x =>
        {
            x.Result = RunResult.Sucessful();
            called = true;
        });

        var args = CommandDataExtensionMethods.Run(command, plugin, reporter);
        Assert.IsTrue(called);
        Assert.AreEqual(1, reporter.CallCount);
        Assert.IsNotNull(args.Result);
        Assert.AreEqual(RunStatus.Sucessful, args.Result.Status);
    }

    [TestMethod]
    public void SatisfiesCondition_WithNullCommandData()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            var value = CommandDataExtensionMethods.SatifiesCondition(null, new VariablesDictionary(), "test");
        });
    }

    [TestMethod]
    public void SatisfiesCondition_InvalidCommandDataName()
    {
        Assert.ThrowsException<ArgumentException>(() =>
        {
            var com = new CommandData("test");
            var value = CommandDataExtensionMethods.SatifiesCondition(com, new VariablesDictionary(), "test");
        });
    }

    [TestMethod]
    public void SatisfiesCondition_NullCommandData()
    {
        Assert.ThrowsException<ArgumentNullException>(() => CommandDataExtensionMethods.SatifiesCondition(null, new VariablesDictionary()));
    }

    [TestMethod]
    public void SatisfiesCondition_ValidEqualityComparison_IgnoreCase()
    {
        var com = new CommandData("condition");
        com.Parameters["value1"] = "one";
        com.Parameters["operator"] = "=";
        com.Parameters["value2"] = "one";
        com.Parameters["ignorecase"] = "true";
        var value = CommandDataExtensionMethods.SatifiesCondition(com, new VariablesDictionary());
        Assert.IsTrue(value);
    }

    [TestMethod]
    public void CommandDataGetParameterWithNullArgsAndDefaultValue()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            CommandData args = null;
            CommandDataExtensionMethods.GetParameter(args, "test", string.Empty);
        });
    }

    [TestMethod]
    public void CommandDataGetParameterWithValidArgsAndNoDefaultValue()
    {
        var args = new CommandData();
        args.Parameters["test"] = "value";
        var value = CommandDataExtensionMethods.GetParameter<string>(args, "test");
        Assert.AreEqual("value", value);
    }

    [TestMethod]
    public void CommandDataGetParameterWithValidArgsAndDefaultValue()
    {
        var args = new CommandData();
        var value = CommandDataExtensionMethods.GetParameter(args, "test", "default");
        Assert.AreEqual("default", value);
    }

    [TestMethod]
    public void SatifiesConditions_JustEmptyVariblesDictionary_ChecksAllConditions()
    {
        var data = "<dummy>\r\n" +
                   "    <condition value1=\"System.Data.xml\" operator=\"startswith\" value2=\"System.\" ignorecase=\"true\" />\r\n" +
                   "    <condition value1=\"System.Data.xml\" operator=\"endswith\" value2=\".xml\" ignorecase=\"true\" />\r\n" +
                   "</dummy>";

        var item = XElement.Parse(data);
        var node = TestHelpers.BuildCommandNode(item, null);
        var variables = new VariablesDictionary();

        var result = node.SatifiesConditions(variables);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void SatifiesConditions_JustEmptyVariblesDictionary_ChecksAllConditions_SpecifiedCompareValue()
    {
        var data = "<dummy>\r\n" +
                   "    <condition operator=\"startswith\" value2=\"System.\" ignorecase=\"true\" />\r\n" +
                   "    <condition operator=\"endswith\" value2=\".xml\" ignorecase=\"true\" />\r\n" +
                   "</dummy>";

        var item = XElement.Parse(data);
        var node = TestHelpers.BuildCommandNode(item, null);
        var variables = new VariablesDictionary();

        var result = node.SatifiesConditions(variables, "System.Data.xml");

        Assert.IsTrue(result);
    }
}