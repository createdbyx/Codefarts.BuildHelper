using System;
using Codefarts.BuildHelper;
using Codefarts.BuildHelper.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildHelperTests;

[TestClass]
[TestCategory("Extension Methods - RunCommandArgs")]
public class RunCommandArgsExtensionMethodsTests
{
    [TestMethod]
    public void GetVariableWithNullArgsAndDefaultValue()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            RunCommandArgs args = null;
            RunCommandArgsExtensionMethods.GetVariable(args, "test", string.Empty);
        });
    }

    [TestMethod]
    public void GetVariableWithNullArgsAndNoDefaultValue()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            RunCommandArgs args = null;
            RunCommandArgsExtensionMethods.GetVariable<string>(args, "test");
        });
    }

    [TestMethod]
    public void GetVariableThatReturnsDefaultValue()
    {
        var args = new RunCommandArgs(new VariablesDictionary(), new CommandData());
        var value = RunCommandArgsExtensionMethods.GetVariable(args, "test", "defaultValue");
        Assert.AreEqual("defaultValue", value);
    }

    [TestMethod]
    public void GetMissingVariableWithoutDefaultValue()
    {
        var args = new RunCommandArgs(new VariablesDictionary(), new CommandData());
        Assert.ThrowsException<MissingVariableException>(() =>
        {
            var value = RunCommandArgsExtensionMethods.GetVariable<string>(args, "test");
        });
    }

    [TestMethod]
    public void GetValidVariableWithoutDefaultValue()
    {
        var variables = new VariablesDictionary();
        variables["test"] = "someValue";
        var args = new RunCommandArgs(variables, new CommandData());

        var value = RunCommandArgsExtensionMethods.GetVariable<string>(args, "test");
        Assert.AreEqual("someValue", value);
    }

    [TestMethod]
    public void GetValidVariableWithBadCastingType()
    {
        var variables = new VariablesDictionary();
        variables["test"] = "value";
        var args = new RunCommandArgs(variables, new CommandData());
        Assert.ThrowsException<FormatException>(() =>
        {
            var value = RunCommandArgsExtensionMethods.GetVariable<int>(args, "test");
        });
    }

    [TestMethod]
    public void GetVariableThatDoesNotImplementIConvertableWithoutDefaultValue()
    {
        Assert.ThrowsException<InvalidCastException>(() =>
        {
            var dic = new VariablesDictionary();
            var com = new CommandData("Command");
            dic["test"] = new Random();
            var args = new RunCommandArgs(dic, com);
            var value = RunCommandArgsExtensionMethods.GetVariable<bool>(args, "test");
            Assert.Fail($"Should have thrown {nameof(InvalidCastException)}.");
        });
    }

    [TestMethod]
    public void GetVariableThatDoesNotImplementIConvertableWithADefaultValue()
    {
        var dic = new VariablesDictionary();
        var com = new CommandData("Command");
        dic["test"] = new Random();
        var args = new RunCommandArgs(dic, com);
        var value = RunCommandArgsExtensionMethods.GetVariable<bool>(args, "test", true);
        Assert.IsTrue(value);
    }

    [TestMethod]
    public void GetVariableThatIsMissingWithADefaultValue()
    {
        var dic = new VariablesDictionary();
        var com = new CommandData("Command");
        dic["test"] = new Random();
        var args = new RunCommandArgs(dic, com);
        var value = RunCommandArgsExtensionMethods.GetVariable<bool>(args, "missing", true);
        Assert.IsTrue(value);
    }

    [TestMethod]
    public void GetVariableThatIsIConvertableWithADefaultValue()
    {
        var dic = new VariablesDictionary();
        var com = new CommandData("Command");
        dic["test"] = 82;
        var args = new RunCommandArgs(dic, com);
        var value = RunCommandArgsExtensionMethods.GetVariable<string>(args, "test", "24");
        Assert.AreEqual("82", value);
    }

    [TestMethod]
    public void RunCommandArgsGetParameterWithNullArgsAndDefaultValue()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            RunCommandArgs args = null;
            RunCommandArgsExtensionMethods.GetParameter(args, "test", string.Empty);
        });
    }

    [TestMethod]
    public void RunCommandArgsGetParameterWithMissingParameterAndDefaultValue()
    {
        var data = new CommandData();
        var args = new RunCommandArgs(new VariablesDictionary(), data);
        var value = RunCommandArgsExtensionMethods.GetParameter(args, "test", "default");
        Assert.AreEqual("default", value);
    }

    [TestMethod]
    public void RunCommandArgsGetParameterWithValidParameterAndNoDefaultValue()
    {
        var data = new CommandData();
        data.Parameters["test"] = "value";
        var args = new RunCommandArgs(new VariablesDictionary(), data);
        var value = RunCommandArgsExtensionMethods.GetParameter<string>(args, "test");
        Assert.AreEqual("value", value);
    }

    [TestMethod]
    public void RunCommandArgsGetParameterWithValidParameterAndNoDefaultValueButBadReturnType()
    {
        var data = new CommandData();
        data.Parameters["test"] = "value";
        var args = new RunCommandArgs(new VariablesDictionary(), data);
        Assert.ThrowsException<FormatException>(() => { RunCommandArgsExtensionMethods.GetParameter<bool>(args, "test"); });
    }

    [TestMethod]
    public void RunCommandArgsGetParameterWithMissingParameterAndNoDefaultValue()
    {
        var data = new CommandData();
        var args = new RunCommandArgs(new VariablesDictionary(), data);
        Assert.ThrowsException<MissingParameterException>(() => { RunCommandArgsExtensionMethods.GetParameter<string>(args, "test"); });
    }

    [TestMethod]
    public void RunCommandArgsGetParameterWithMissingArgsAndNoDefaultValue()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            RunCommandArgs args = null;
            RunCommandArgsExtensionMethods.GetParameter<string>(args, "test");
        });
    }

    [TestMethod]
    public void GetParameterThatDoesNotImplementIConvertableWithoutDefaultValue()
    {
        Assert.ThrowsException<InvalidCastException>(() =>
        {
            var dic = new VariablesDictionary();
            var com = new CommandData("Command");
            com.Parameters["test"] = new Random();
            var args = new RunCommandArgs(dic, com);
            var value = RunCommandArgsExtensionMethods.GetParameter<bool>(args, "test");
            Assert.Fail($"Should have thrown {nameof(InvalidCastException)}.");
        });
    }
}