using System.Collections.Generic;
using BuildHelperTests.Mocks;
using Codefarts.BuildHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildHelperTests;

[TestClass]
[TestCategory("Extension Methods - IConfigurationProvider")]
public class IConfigurationProviderExtensionMethods
{
    [TestMethod]
    public void GenericGetValue_GetValidValue()
    {
        var config = new MockConfigProvider();
        config.SetValue("test", "System.Data.xml");

        var result = config.GetValue<string>("test", "nope");
        Assert.AreEqual("System.Data.xml", result);
    }
    
    [TestMethod]
    public void GenericGetValue_GetBadKeyName()
    {
        var config = new MockConfigProvider();
        config.SetValue("test", "System.Data.xml");

        Assert.ThrowsException<KeyNotFoundException>(() => config.GetValue<string>("badkey"));
    }
}