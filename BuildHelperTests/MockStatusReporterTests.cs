using BuildHelperTests.Mocks;
using Codefarts.BuildHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildHelperTests;

[TestClass]
[TestCategory("MockStatusReporter")]
public class MockStatusReporterTests
{

    [TestMethod]
    public void ReportErrorResultJustMessage()
    {
        var report = new MockStatusReporter();
        report.ReportError("test");
        Assert.AreEqual("test", report.Message);
        Assert.IsNull(report.Category);
        Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Message));
        Assert.IsFalse(report.Type.HasFlag(ReportStatusType.Progress));
        Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Error));
    }

    [TestMethod]
    public void ReportErrorResultWithMessageAndProgress()
    {
        var report = new MockStatusReporter();
        report.ReportError("test", 25);
        Assert.AreEqual("test", report.Message);
        Assert.IsNull(report.Category);
        Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Message));
        Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Progress));
        Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Error));
    }

    [TestMethod]
    public void ReportErrorResultWithMessageCategoryProgress()
    {
        var report = new MockStatusReporter();
        report.ReportError("test", "cat", 25);
        Assert.AreEqual("test", report.Message);
        Assert.AreEqual("cat", report.Category);
        Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Message));
        Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Progress));
        Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Error));
    }

    [TestMethod]
    public void ReportProgressResultWithMessageProgress()
    {
        var report = new MockStatusReporter();
        report.ReportProgress("test", 25);
        Assert.AreEqual("test", report.Message);
        Assert.IsNull(report.Category);
        Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Message));
        Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Progress));
        Assert.IsFalse(report.Type.HasFlag(ReportStatusType.Error));
    }

    [TestMethod]
    public void ReportProgressResultWithMessageCategoryProgress()
    {
        var report = new MockStatusReporter();
        report.ReportProgress("test", "cat", 25);
        Assert.AreEqual("test", report.Message);
        Assert.AreEqual("cat", report.Category);
        Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Message));
        Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Progress));
        Assert.IsFalse(report.Type.HasFlag(ReportStatusType.Error));
    }

    [TestMethod]
    public void ReportProgressResultWithProgress()
    {
        var report = new MockStatusReporter();
        report.ReportProgress(25);
        Assert.IsNull(report.Message);
        Assert.IsNull(report.Category);
        Assert.IsFalse(report.Type.HasFlag(ReportStatusType.Message));
        Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Progress));
        Assert.IsFalse(report.Type.HasFlag(ReportStatusType.Error));
    }

    [TestMethod]
    public void ReportMessage()
    {
        var report = new MockStatusReporter();
        report.Report("test");
        Assert.AreEqual("test", report.Message);
        Assert.IsNull(report.Category);
        Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Message));
        Assert.IsFalse(report.Type.HasFlag(ReportStatusType.Progress));
        Assert.IsFalse(report.Type.HasFlag(ReportStatusType.Error));
    }

    [TestMethod]
    public void ReportMessageCategory()
    {
        var report = new MockStatusReporter();
        report.Report("test", "cat");
        Assert.AreEqual("test", report.Message);
        Assert.AreEqual("cat", report.Category);
        Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Message));
        Assert.IsFalse(report.Type.HasFlag(ReportStatusType.Progress));
        Assert.IsFalse(report.Type.HasFlag(ReportStatusType.Error));
    }
}