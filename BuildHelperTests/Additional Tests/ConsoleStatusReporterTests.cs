// <copyright file="ConsoleStatusReporterTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using Codefarts.BuildHelper.ConsoleReporter;

namespace BuildHelperTests.Additional_Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass, TestCategory(nameof(ConsoleStatusReporter) + " Command")]
public class ConsoleStatusReporterTests
{
    [TestInitialize]
    public void InitTest()
    {
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }

    [TestMethod]
    public void DefaultHeaderPrefix()
    {
        var command = new ConsoleStatusReporter();
        Assert.IsNull(command.HeaderPrefix);
    }

    [TestMethod]
    public void SetHeaderPrefix()
    {
        var command = new ConsoleStatusReporter();
        command.HeaderPrefix = "test";
        Assert.AreEqual("test", command.HeaderPrefix);
    }
}