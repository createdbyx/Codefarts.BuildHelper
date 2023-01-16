using System;
using BuildHelperTests.Mocks;
using Codefarts.BuildHelper;
using Codefarts.BuildHelperConsoleApp;
using Codefarts.DependencyInjection;
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

    [TestMethod]
    public void Run_ValidArguments_NoImporter()
    {
        var ioc = new DependencyInjectorShim(new Container());
        var app = new Application(ioc);
        Assert.ThrowsException<ContainerResolutionException>(() => app.Run());
    }

    [TestMethod]
    public void Run_ValidArguments_WithImporter_Succesful()
    {
        var ioc = new DependencyInjectorShim(new Container());
        ioc.Register<IDependencyInjectionProvider>(() => ioc);
        var app = new Application(ioc);
        ioc.Register<ICommandImporter, MockSuccsesfulCommandImporter>();

        var result = app.Run();
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Error);
        Assert.IsNull(result.ReturnValue);
    }
}