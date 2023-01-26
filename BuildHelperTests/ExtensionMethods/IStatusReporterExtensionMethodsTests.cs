using System;
using Codefarts.BuildHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildHelperTests;

[TestClass]
[TestCategory("Extension Methods - IStatusReporter")]
public class IStatusReporterExtensionMethodsTests
{
    [TestMethod]
    public void ReportErrorResultJustMessageWithNullStatus()
    {
        Assert.ThrowsException<ArgumentNullException>(() => IStatusReporterExtensionMethods.ReportError(null, "test"));
    }
}