// <copyright file="CopyDirCommandTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace BuildHelperTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Codefarts.BuildHelper;
    using Codefats.BuildHelper.ConsoleReporter;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, TestCategory("CopyDir Command")]
    public class CopyDirCommandTests
    {
        private VariablesDictionary variables;
        private string tempDir;
        private string destDir;

        [TestInitialize]
        public void InitTest()
        {
            this.tempDir = Path.Combine(Path.GetTempPath(), "CopyDirTest_" + Guid.NewGuid().ToString("N"));
            this.destDir = Path.Combine(Path.GetTempPath(), "CopyDirDestinationTest_" + Guid.NewGuid().ToString("N"));
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
            this.variables["DestPath"] = this.destDir;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Directory.Delete(this.tempDir, true);
            if (Directory.Exists(this.destDir))
            {
                Directory.Delete(this.destDir, true);
            }

            this.variables = null;
        }

        [TestMethod]
        public void ValidateCommandName()
        {
            var command = new CopyDirCommand();
            Assert.AreEqual("copydir", command.Name);
        }

        [TestMethod]
        public void NullStatusThrowsException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CopyDirCommand(null));
        }

        [TestMethod]
        public void NullArgs()
        {
            var command = new CopyDirCommand();
            Assert.ThrowsException<ArgumentNullException>(() => command.Run(null));
        }

        [TestMethod]
        public void WithValidStatusArg()
        {
            var status = new ConsoleStatusReporter();
            var command = new CopyDirCommand(status);
        }

        [TestMethod]
        public void NoConditionsOrAdditionalParameters()
        {
            var command = new CopyDirCommand();
            var data = "<copydir source=\"$(TempPath)\" destination=\"$(DestPath)\" />";

            var item = XElement.Parse(data);
            var buildFileCommand = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, buildFileCommand);

            command.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Sucessful, args.Result.Status);
            Assert.IsNull(args.Result.Error);

            var fileCount = Directory.GetFiles(this.destDir, "*.*", SearchOption.AllDirectories).Length;
            Assert.AreEqual(5, fileCount);
        }

        [TestMethod]
        public void NoConditionsWithCleanParameter()
        {
            var command = new CopyDirCommand();
            var data = "<copydir source=\"$(TempPath)\" destination=\"$(DestPath)\" clean=\"true\" />";

            var item = XElement.Parse(data);
            var buildFileCommand = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, buildFileCommand);

            // create dest folder
            Directory.CreateDirectory(this.destDir);

            // create dest files to be cleared
            var cleanFile = Path.Combine(this.destDir, "clean.txt");
            File.WriteAllText(cleanFile, "contents");

            var cleanDir = Path.Combine(this.destDir, "CleanSubFolder");
            Directory.CreateDirectory(cleanDir);

            command.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Sucessful, args.Result.Status);
            Assert.IsNull(args.Result.Error);

            // ensure clear ile does not exist
            Assert.IsFalse(File.Exists(cleanFile));
            Assert.IsFalse(Directory.Exists(cleanDir));

            var fileCount = Directory.GetFiles(this.destDir, "*.*", SearchOption.AllDirectories).Length;
            Assert.AreEqual(5, fileCount);
        }

        [TestMethod]
        public void NoConditionsWithSubFoldersFalse()
        {
            var command = new CopyDirCommand();
            var data = "<copydir source=\"$(TempPath)\" destination=\"$(DestPath)\"  subfolders=\"false\" />";

            var item = XElement.Parse(data);
            var buildFileCommand = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, buildFileCommand);

            // create dest folder
            Directory.CreateDirectory(this.destDir);

            command.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Sucessful, args.Result.Status);
            Assert.IsNull(args.Result.Error);

            var files = Directory.GetFiles(this.destDir, "*.*", SearchOption.AllDirectories).OrderBy(x => x).ToArray();
            Assert.AreEqual(3, files.Length);
            Assert.AreEqual("File1.txt", Path.GetFileName(files[0]));
            Assert.AreEqual("File2.xml", Path.GetFileName(files[1]));
            Assert.AreEqual("System.File3.dat", Path.GetFileName(files[2]));
        }

        [TestMethod]
        public void NoConditionsWithLockedSourceFile()
        {
            var command = new CopyDirCommand();
            var data = "<copydir source=\"$(TempPath)\" destination=\"$(DestPath)\" clean=\"true\" />";

            var item = XElement.Parse(data);
            var buildFileCommand = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, buildFileCommand);

            var file1 = Path.Combine(this.tempDir, "File1.txt");
            using (var stream = new FileStream(file1, FileMode.Open, FileAccess.Write, FileShare.Write))
            {
                command.Run(args);
            }

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            Assert.IsNotNull(args.Result.Error);

            var ioe = args.Result.Error as IOException;
            Assert.IsNotNull(ioe);

            // dest dir should exists
            Assert.IsTrue(Directory.Exists(this.destDir));

            // no files should have been copied
            var fileCount = Directory.GetFiles(this.destDir, "*.*", SearchOption.AllDirectories).Length;
            Assert.AreEqual(0, fileCount);
        }

        [TestMethod]
        public void NoConditionsWithCleanParameterSetToWhitespace()
        {
            var command = new CopyDirCommand();
            var data = "<copydir source=\"$(TempPath)\" destination=\"$(DestPath)\" clean=\" \" />";

            var item = XElement.Parse(data);
            var buildFileCommand = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, buildFileCommand);

            // create dest folder
            Directory.CreateDirectory(this.destDir);

            // create dest files to be cleared
            var cleanFile = Path.Combine(this.destDir, "clean.txt");
            File.WriteAllText(cleanFile, "contents");

            command.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Sucessful, args.Result.Status);
            Assert.IsNull(args.Result.Error);

            // ensure clear ile does not exist
            Assert.IsTrue(File.Exists(cleanFile));

            var fileCount = Directory.GetFiles(this.destDir, "*.*", SearchOption.AllDirectories).Length;
            Assert.AreEqual(6, fileCount);
        }

        [TestMethod]
        public void NoConditionsSourceIsWhitespace()
        {
            var command = new CopyDirCommand();
            var data = "<copydir source=\"  \" destination=\"$(DestPath)\" clean=\"true\" />";

            var item = XElement.Parse(data);
            var buildFileCommand = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, buildFileCommand);

            command.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            Assert.IsNotNull(args.Result.Error);

            var mpe = args.Result.Error as MissingParameterException;
            Assert.IsNotNull(mpe);
            Assert.AreEqual("source", mpe.ParameterName);
        }

        [TestMethod]
        public void NoConditionsSourceIsMissing()
        {
            var command = new CopyDirCommand();
            var data = "<copydir destination=\"$(DestPath)\" clean=\"true\" />";

            var item = XElement.Parse(data);
            var buildFileCommand = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, buildFileCommand);

            command.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            Assert.IsNotNull(args.Result.Error);

            var mpe = args.Result.Error as MissingParameterException;
            Assert.IsNotNull(mpe);
            Assert.AreEqual("source", mpe.ParameterName);
        }

        [TestMethod]
        public void NoConditionsDestinationIsWhitespace()
        {
            var command = new CopyDirCommand();
            var data = "<copydir source=\"$(TempPath)\" destination=\"  \" clean=\"true\" />";

            var item = XElement.Parse(data);
            var buildFileCommand = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, buildFileCommand);

            command.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            Assert.IsNotNull(args.Result.Error);

            var mpe = args.Result.Error as MissingParameterException;
            Assert.IsNotNull(mpe);
            Assert.AreEqual("destination", mpe.ParameterName);
        }

        [TestMethod]
        public void NoConditionsDestinationIsMissing()
        {
            var command = new CopyDirCommand();
            var data =
                "<copydir source=\"$(TempPath)\" clean=\"true\" >\r\n" +
                // "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"System.\" ignorecase=\"true\" />\r\n" +
                // "    <condition value1=\"$(PurgeEntry)\" operator=\"startswith\" value2=\"Microsoft.\" ignorecase=\"true\" />\r\n" +
                // "    <condition value1=\"$(PurgeEntry)\" operator=\"endswith\" value2=\".xml\" ignorecase=\"true\" />\r\n" +
                "</copydir>";

            var item = XElement.Parse(data);
            var buildFileCommand = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, buildFileCommand);

            command.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            Assert.IsNotNull(args.Result.Error);

            var mpe = args.Result.Error as MissingParameterException;
            Assert.IsNotNull(mpe);
            Assert.AreEqual("destination", mpe.ParameterName);
        }
    }
}