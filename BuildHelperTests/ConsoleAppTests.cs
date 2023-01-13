using System;
using Codefarts.BuildHelperConsoleApp;
using Codefarts.IoC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildHelperTests;

[TestClass]
[TestCategory("Console Application")]
public class ConsoleAppTests
{
    [TestMethod]
    public void Ctor_NullArguments()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            var app = new Application(null);
        });
    }

    [TestMethod]
    public void Ctor_ValidArguments()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var app = new Application(ioc);
        Assert.IsNotNull(app);
    }
}