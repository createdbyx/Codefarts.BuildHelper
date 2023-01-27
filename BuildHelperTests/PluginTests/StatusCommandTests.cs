// <copyright file="StatusCommandTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using BuildHelperTests.Mocks;

namespace BuildHelperTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Codefarts.BuildHelper;
    using Codefats.BuildHelper.ConsoleReporter;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, TestCategory("Status Command")]
    public class StatusCommandTests
    {
        private VariablesDictionary variables;
        private string tempDir;
        private string destDir;

        [TestInitialize]
        public void InitTest()
        {
            // this.tempDir = Path.Combine(Path.GetTempPath(), "CopyDirTest_" + Guid.NewGuid().ToString("N"));
            // this.destDir = Path.Combine(Path.GetTempPath(), "CopyDirDestinationTest_" + Guid.NewGuid().ToString("N"));
            // Directory.CreateDirectory(this.tempDir);
            // File.WriteAllText(Path.Combine(this.tempDir, "File1.txt"), "File1Data"); // \File1.txt
            // File.WriteAllText(Path.Combine(this.tempDir, "File2.xml"), "File2Data"); // \File2.xml
            // File.WriteAllText(Path.Combine(this.tempDir, "System.File3.dat"), "File3Data"); // \System.File3.dat
            // Directory.CreateDirectory(Path.Combine(this.tempDir, "SubFolder")); // \SubFolder\
            // Directory.CreateDirectory(Path.Combine(this.tempDir, "SubFolder", "Sub2")); // \SubFolder\Sub2\
            // File.WriteAllText(Path.Combine(this.tempDir, "SubFolder", "Microsoft.File4.db"), "File4Data"); // \SubFolder\Microsoft.File4.db
            // File.WriteAllText(Path.Combine(this.tempDir, "SubFolder", "Sub2", "Taxi.File5.pdb"), "File5Data"); // \SubFolder\Sub2\Taxi.File5.pdb
            this.variables = new VariablesDictionary();
            //this.variables["TempPath"] = this.tempDir;
            //this.variables["DestPath"] = this.destDir;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Directory.Delete(this.tempDir, true);
            // if (Directory.Exists(this.destDir))
            // {
            //     Directory.Delete(this.destDir, true);
            // }

            this.variables = null;
        }

        [TestMethod]
        public void ValidateCommandName()
        {
            var reporter = new ConsoleStatusReporter();
            var command = new StatusCommand(reporter);
            Assert.AreEqual("status", command.Name);
        }

        [TestMethod]
        public void NullStatusThrowsException()
        {
            var com = new StatusCommand(null);
            Assert.IsNotNull(com);
        }

        [TestMethod]
        public void CtorNoArgs()
        {
            var command = new StatusCommand();
        }

        [TestMethod]
        public void NullRunArgs()
        {
            var reporter = new ConsoleStatusReporter();
            var command = new StatusCommand(reporter);
            Assert.ThrowsException<ArgumentNullException>(() => command.Run(null));
        }

        [TestMethod]
        public void SimpleMessage()
        {
            var reporter = new MockStatusReporter();
            var command = new StatusCommand(reporter);
            var data = "<status text=\"test\" />";

            var item = XElement.Parse(data);
            var node = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, node);

            command.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Sucessful, args.Result.Status);
            Assert.IsNull(args.Result.Error);

            Assert.AreEqual(1, reporter.CallCount);
            Assert.AreEqual("test", reporter.Message);
            Assert.AreEqual(ReportStatusType.Message, reporter.Type);
            Assert.AreEqual(null, reporter.Category);
            Assert.AreEqual(0f, reporter.Progress);
        }
        
        [TestMethod]
        public void BadCommandName()
        {
            var reporter = new MockStatusReporter();
            var command = new StatusCommand(reporter);
            var data = "<badstatusname text=\"test\" />";

            var item = XElement.Parse(data);
            var node = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, node);

            command.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            Assert.IsNotNull(args.Result.Error);
            Assert.IsInstanceOfType<ArgumentException>(args.Result.Error);
        }

        [TestMethod]
        public void ComplexMessage()
        {
            var reporter = new MockStatusReporter();
            var command = new StatusCommand(reporter);
            var data = "<status text=\"test\" type=\"Progress\" category=\"Cat\" progress=\"25\" />";

            var item = XElement.Parse(data);
            var node = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, node);

            command.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Sucessful, args.Result.Status);
            Assert.IsNull(args.Result.Error);

            Assert.AreEqual(1, reporter.CallCount);
            Assert.AreEqual("test", reporter.Message);
            Assert.AreEqual(ReportStatusType.Progress, reporter.Type);
            Assert.AreEqual("Cat", reporter.Category);
            Assert.AreEqual(25f, reporter.Progress);
        }

        [TestMethod]
        public void MultipleEnums_CommaSeparated()
        {
            var reporter = new MockStatusReporter();
            var command = new StatusCommand(reporter);
            var data = "<status text=\"test\" type=\"Progress, Message\" category=\"Cat\" progress=\"25\" />";

            var item = XElement.Parse(data);
            var node = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, node);

            command.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Sucessful, args.Result.Status);
            Assert.IsNull(args.Result.Error);

            Assert.AreEqual(1, reporter.CallCount);
            Assert.AreEqual("test", reporter.Message);
            Assert.IsTrue(Enum.IsDefined(typeof(ReportStatusType), ReportStatusType.Progress));
            Assert.IsTrue(Enum.IsDefined(typeof(ReportStatusType), ReportStatusType.Message));
            Assert.AreEqual("Cat", reporter.Category);
            Assert.AreEqual(25f, reporter.Progress);
        }

        [TestMethod]
        public void MultipleEnums_PipeSeparated()
        {
            var reporter = new MockStatusReporter();
            var command = new StatusCommand(reporter);
            var data = "<status text=\"test\" type=\"Progress | Message\" category=\"Cat\" progress=\"25\" />";

            var item = XElement.Parse(data);
            var node = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, node);

            command.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Sucessful, args.Result.Status);
            Assert.IsNull(args.Result.Error);

            Assert.AreEqual(1, reporter.CallCount);
            Assert.AreEqual("test", reporter.Message);
            Assert.IsTrue(Enum.IsDefined(typeof(ReportStatusType), ReportStatusType.Progress));
            Assert.IsTrue(Enum.IsDefined(typeof(ReportStatusType), ReportStatusType.Message));
            Assert.AreEqual("Cat", reporter.Category);
            Assert.AreEqual(25f, reporter.Progress);
        }
    }
}