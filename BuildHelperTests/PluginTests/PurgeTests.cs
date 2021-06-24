// <copyright file="PurgeTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace BuildHelperTests
{
    using System;
    using System.IO;
    using System.Xml.Linq;
    using Codefarts.BuildHelper;
    using Codefats.BuildHelper.ConsoleReporter;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, TestCategory("Purge Command")]
    public class PurgeFiles
    {
        private VariablesDictionary variables;
        private string tempDir;

        [TestInitialize]
        public void InitTest()
        {
            this.tempDir = Path.Combine(Path.GetTempPath(), "PurgeFilesTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(this.tempDir);
            File.WriteAllText(Path.Combine(this.tempDir, "File1.txt"), "File1Data"); // \File1.txt
            File.WriteAllText(Path.Combine(this.tempDir, "File2.xml"), "File2Data"); // \File2.xml
            File.WriteAllText(Path.Combine(this.tempDir, "System.File3.dat"), "File3Data"); // \System.File3.dat
            Directory.CreateDirectory(Path.Combine(this.tempDir, "SubFolder")); // \SubFolder\
            Directory.CreateDirectory(Path.Combine(this.tempDir, "SubFolder", "Sub2")); // \SubFolder\Sub2\
            File.WriteAllText(Path.Combine(this.tempDir, "SubFolder", "Microsoft.File4.db"), "File4Data"); // \SubFolder\Microsoft.File4.db
            File.WriteAllText(Path.Combine(this.tempDir, "SubFolder", "Sub2", "Taxi.File5.pdb"), "File5Data"); // \SubFolder\Sub2\Taxi.File5.pdb
            this.variables = new VariablesDictionary();
            this.variables["TempPath"] = this.tempDir;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Directory.Delete(tempDir, true);
            this.variables = null;
        }

        [TestMethod]
        public void ValidateCommandName()
        {
            var purge = new PurgeCommand();
            Assert.AreEqual("purge", purge.Name);
        }

        [TestMethod]
        public void NullStatusThrowsException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PurgeCommand(null));
        }

        [TestMethod]
        public void NullArgs()
        {
            var purge = new PurgeCommand();
            Assert.ThrowsException<ArgumentNullException>(() => purge.Run(null));
        }

        [TestMethod]
        public void WithValidStatusArg()
        {
            var status = new ConsoleStatusReporter();
            var purge = new PurgeCommand(status);
        }

        [TestMethod]
        public void TryPurgeReadOnlyFile()
        {
            var purge = new PurgeCommand();
            var data =
                "<purge ignoreconditions=\"true\" path=\"$(TempPath)\" subfolders=\"true\" allconditions=\"false\" fullpaths=\"false\" type=\"files\">\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"System.\" ignorecase=\"true\" />\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"Microsoft.\" ignorecase=\"true\" />\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"endswith\" value2=\".xml\" ignorecase=\"true\" />\r\n" +
                "</purge>";

            var item = XElement.Parse(data);
            var buildFileCommand = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, buildFileCommand);

            // make first file readonly
            var firstFile = new FileInfo(Path.Combine(this.tempDir, "File1.txt"));
            firstFile.IsReadOnly = true;

            purge.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            Assert.IsNotNull(args.Result.Error);

            var uae = args.Result.Error as UnauthorizedAccessException;
            Assert.IsNotNull(uae);

            // remove readonly so cleanup can occur
            firstFile.IsReadOnly = false;
        }

        [TestMethod]
        public void IgnoreConditionsSetTrue()
        {
            var purge = new PurgeCommand();
            var data =
                "<purge ignoreconditions=\"true\" path=\"$(TempPath)\" subfolders=\"true\" allconditions=\"false\" fullpaths=\"false\" type=\"files\">\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"System.\" ignorecase=\"true\" />\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"Microsoft.\" ignorecase=\"true\" />\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"endswith\" value2=\".xml\" ignorecase=\"true\" />\r\n" +
                "</purge>";

            var item = XElement.Parse(data);
            var buildFileCommand = TestHelpers.BuildCommandNode(item, null);
            purge.Run(new RunCommandArgs(this.variables, buildFileCommand));

            var files = Directory.GetFiles(this.tempDir, "*.*", SearchOption.AllDirectories);
            var folders = Directory.GetDirectories(this.tempDir, "*.*", SearchOption.AllDirectories);
            Assert.AreEqual(0, files.Length);
            Assert.AreEqual(2, folders.Length);
        }

        [TestMethod]
        public void MissingPathParameter()
        {
            var purge = new PurgeCommand();
            var data =
                "<purge subfolders=\"true\" allconditions=\"false\" fullpaths=\"false\" type=\"files\">\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"System.\" ignorecase=\"true\" />\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"Microsoft.\" ignorecase=\"true\" />\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"endswith\" value2=\".xml\" ignorecase=\"true\" />\r\n" +
                "</purge>";

            var item = XElement.Parse(data);
            var buildFileCommand = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, buildFileCommand);
            purge.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            Assert.AreSame(typeof(MissingParameterException), args.Result.Error.GetType());

            var mpe = args.Result.Error as MissingParameterException;
            Assert.AreEqual("path", mpe.ParameterName);
        }

        [TestMethod]
        public void MissingTypeParameter()
        {
            var purge = new PurgeCommand();
            var data =
                "<purge subfolders=\"true\" path=\"$(TempPath)\" allconditions=\"false\" fullpaths=\"false\" >\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"System.\" ignorecase=\"true\" />\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"Microsoft.\" ignorecase=\"true\" />\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"endswith\" value2=\".xml\" ignorecase=\"true\" />\r\n" +
                "</purge>";

            var item = XElement.Parse(data);
            var buildFileCommand = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, buildFileCommand);
            purge.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            Assert.AreSame(typeof(MissingParameterException), args.Result.Error.GetType());

            var mpe = args.Result.Error as MissingParameterException;
            Assert.AreEqual("type", mpe.ParameterName);
        }

        [TestMethod]
        public void PathParameterNoValue()
        {
            var purge = new PurgeCommand();
            var data =
                "<purge subfolders=\"true\" path=\"\" allconditions=\"false\" fullpaths=\"false\" type=\"files\">\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"System.\" ignorecase=\"true\" />\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"Microsoft.\" ignorecase=\"true\" />\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"endswith\" value2=\".xml\" ignorecase=\"true\" />\r\n" +
                "</purge>";

            var item = XElement.Parse(data);
            var buildFileCommand = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, buildFileCommand);
            purge.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            Assert.AreSame(typeof(MissingParameterException), args.Result.Error.GetType());

            var mpe = args.Result.Error as MissingParameterException;
            Assert.AreEqual("path", mpe.ParameterName);
        }

        [TestMethod]
        public void TypeParameterNoValue()
        {
            var purge = new PurgeCommand();
            var data =
                "<purge subfolders=\"true\" path=\"$(TempPath)\" allconditions=\"false\" fullpaths=\"false\" type=\"\">\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"System.\" ignorecase=\"true\" />\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"Microsoft.\" ignorecase=\"true\" />\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"endswith\" value2=\".xml\" ignorecase=\"true\" />\r\n" +
                "</purge>";

            var item = XElement.Parse(data);
            var buildFileCommand = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, buildFileCommand);
            purge.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            Assert.AreSame(typeof(BuildException), args.Result.Error.GetType());

            var buildException = args.Result.Error as BuildException;
            Assert.IsNotNull(buildException);
        }

        [TestMethod]
        public void TypeParameterBadValue()
        {
            var purge = new PurgeCommand();
            var data =
                "<purge subfolders=\"true\" path=\"$(TempPath)\" allconditions=\"false\" fullpaths=\"false\" type=\"badValue\">\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"System.\" ignorecase=\"true\" />\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"Microsoft.\" ignorecase=\"true\" />\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"endswith\" value2=\".xml\" ignorecase=\"true\" />\r\n" +
                "</purge>";

            var item = XElement.Parse(data);
            var buildFileCommand = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, buildFileCommand);
            purge.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            Assert.AreSame(typeof(BuildException), args.Result.Error.GetType());

            var mpe = args.Result.Error as BuildException;
            Assert.IsNotNull(mpe);
        }

        [TestMethod]
        public void PathParameterPathDoesNotExist()
        {
            var purge = new PurgeCommand();
            var data =
                "<purge subfolders=\"true\" path=\"z:\\someNonExistent\\Path\" allconditions=\"false\" fullpaths=\"false\" type=\"files\">\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"System.\" ignorecase=\"true\" />\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"Microsoft.\" ignorecase=\"true\" />\r\n" +
                "    <condition value1=\"$(PurgeEntry)\" operator=\"endswith\" value2=\".xml\" ignorecase=\"true\" />\r\n" +
                "</purge>";

            var item = XElement.Parse(data);
            var buildFileCommand = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, buildFileCommand);
            purge.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            Assert.AreSame(typeof(DirectoryNotFoundException), args.Result.Error.GetType());

            var dnf = args.Result.Error as DirectoryNotFoundException;
            Assert.IsNotNull(dnf);
        }

        [TestMethod]
        public void Purge2Files()
        {
            var purge = new PurgeCommand();
            var data = "<purge path=\"$(TempPath)\" subfolders=\"true\" allconditions=\"false\" fullpaths=\"false\" type=\"files\">\r\n" +
                       "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"System.\" ignorecase=\"true\" />\r\n" +
                       "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"Microsoft.\" ignorecase=\"true\" />\r\n" +
                       "    <condition value1=\"$(PurgeEntry)\" operator=\"endswith\" value2=\".xml\" ignorecase=\"true\" />\r\n" +
                       "</purge>";

            var item = XElement.Parse(data);
            var buildFileCommand = TestHelpers.BuildCommandNode(item, null);
            purge.Run(new RunCommandArgs(this.variables, buildFileCommand));
            Assert.IsTrue(File.Exists(Path.Combine(this.tempDir, "File1.txt")));
            Assert.IsFalse(File.Exists(Path.Combine(this.tempDir, "File2.xml")));
            Assert.IsFalse(File.Exists(Path.Combine(this.tempDir, "System.File3.dat")));
            Assert.IsFalse(File.Exists(Path.Combine(this.tempDir, "SubFolder", "Microsoft.File4.db")));
            Assert.IsTrue(File.Exists(Path.Combine(this.tempDir, "SubFolder", "Sub2", "Taxi.File5.pdb")));
        }

        [TestMethod]
        public void Purge2Files_FullPathsTrue()
        {
            var purge = new PurgeCommand();
            var data = "<purge path=\"$(TempPath)\" subfolders=\"true\" allconditions=\"false\" fullpaths=\"true\" type=\"files\">\r\n" +
                       "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"System.\" ignorecase=\"true\" />\r\n" +
                       "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"Microsoft.\" ignorecase=\"true\" />\r\n" +
                       "    <condition value1=\"$(PurgeEntry)\" operator=\"endswith\" value2=\".xml\" ignorecase=\"true\" />\r\n" +
                       "</purge>";

            var item = XElement.Parse(data);
            var buildFileCommand = TestHelpers.BuildCommandNode(item, null);
            purge.Run(new RunCommandArgs(this.variables, buildFileCommand));
            Assert.IsTrue(File.Exists(Path.Combine(this.tempDir, "File1.txt")));
            Assert.IsFalse(File.Exists(Path.Combine(this.tempDir, "File2.xml")));
            Assert.IsTrue(File.Exists(Path.Combine(this.tempDir, "System.File3.dat")));
            Assert.IsTrue(File.Exists(Path.Combine(this.tempDir, "SubFolder", "Microsoft.File4.db")));
            Assert.IsTrue(File.Exists(Path.Combine(this.tempDir, "SubFolder", "Sub2", "Taxi.File5.pdb")));
        }

        [TestMethod]
        public void PurgeSubFolder()
        {
            var purge = new PurgeCommand();
            var data = "<purge path=\"$(TempPath)\" subfolders=\"true\" allconditions=\"false\" fullpaths=\"false\" type=\"folders\">\r\n" +
                       "    <condition value1=\"$(PurgeEntry)\" operator=\"Contains\" value2=\"Folder\" ignorecase=\"false\" />\r\n" +
                       "</purge>";

            var item = XElement.Parse(data);
            var buildFileCommand = TestHelpers.BuildCommandNode(item, null);
            purge.Run(new RunCommandArgs(this.variables, buildFileCommand));
            Assert.IsTrue(File.Exists(Path.Combine(this.tempDir, "File1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(this.tempDir, "File2.xml")));
            Assert.IsTrue(File.Exists(Path.Combine(this.tempDir, "System.File3.dat")));
            Assert.IsFalse(File.Exists(Path.Combine(this.tempDir, "SubFolder", "Microsoft.File4.db")));
            Assert.IsFalse(File.Exists(Path.Combine(this.tempDir, "SubFolder", "Sub2", "Taxi.File5.pdb")));
            Assert.IsFalse(Directory.Exists(Path.Combine(this.tempDir, "SubFolder")));
        }
    }
}