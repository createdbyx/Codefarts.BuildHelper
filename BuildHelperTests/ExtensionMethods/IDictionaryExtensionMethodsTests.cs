using System;
using System.Collections.Generic;
using Codefarts.BuildHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildHelperTests;

[TestClass]
[TestCategory("Extension Methods - IDictionary")]
public class IDictionaryExtensionMethodsTests
{
    [TestMethod]
    public void GetValueWithNullParameters()
    {
        Assert.ThrowsException<ArgumentNullException>(() => { IDictionaryExtensionMethods.GetValue(null, "test", string.Empty); });
    }

    [TestMethod]
    public void GetValueWithNullArgsAndDefaultValue()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            IDictionary<string, object> dic = null;
            IDictionaryExtensionMethods.GetValue(dic, "test", string.Empty);
        });
    }

    [TestMethod]
    public void GetValueThatDoesNotImplementIConvertableWithoutDefaultValue()
    {
        Assert.ThrowsException<InvalidCastException>(() =>
        {
            var dic = new VariablesDictionary();
            dic["test"] = new CommandData("Command");
            var value = IDictionaryExtensionMethods.GetValue<bool>(dic, "test");
            Assert.Fail($"Should have thrown {nameof(InvalidCastException)}.");
        });
    }

    [TestMethod]
    public void GetValueWithNullArgsAndNoDefaultValue()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            IDictionary<string, object> dic = null;
            IDictionaryExtensionMethods.GetValue<string>(dic, "test");
        });
    }

    [TestMethod]
    public void GetValueWithValidArgsAndNoDefaultValue()
    {
        Assert.ThrowsException<KeyNotFoundException>(() =>
        {
            var dic = new Dictionary<string, object>();
            IDictionaryExtensionMethods.GetValue<string>(dic, "test");
        });
    }

    [TestMethod]
    public void GetValueWithValidArgsAndDefaultValue()
    {
        var dic = new Dictionary<string, object>();
        var value = IDictionaryExtensionMethods.GetValue(dic, "test", "default");
        Assert.AreEqual("default", value);
    }

    [TestMethod]
    public void GetValueWithValidArgsExistingKeyAndNoDefaultValue()
    {
        var dic = new Dictionary<string, object>();
        dic["test"] = "value";
        var value = IDictionaryExtensionMethods.GetValue<string>(dic, "test");
        Assert.AreEqual("value", value);
    }

    [TestMethod]
    public void GetValueWithValidArgsExistingKeyAndNoDefaultValueBadCasting()
    {
        var dic = new Dictionary<string, object>();
        dic["test"] = "value";
        Assert.ThrowsException<FormatException>(() => { IDictionaryExtensionMethods.GetValue<bool>(dic, "test"); });
    }
}