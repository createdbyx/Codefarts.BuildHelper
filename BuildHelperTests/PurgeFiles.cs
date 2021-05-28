// <copyright file="PurgeFiles.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace BuildHelperTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;
    using Codefarts.BuildHelper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, TestCategory("Build Commands")]
    public class PurgeFiles
    {
        private IDictionary<string, string> varibles;
        private string tempDir;

        [TestInitialize]
        public void InitTest()
        {
            this.tempDir = Path.Combine(Path.GetTempPath(), "PurgeFilesTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(this.tempDir);
            File.WriteAllText(Path.Combine(this.tempDir, "File1.txt"), "File1Data");
            File.WriteAllText(Path.Combine(this.tempDir, "File2.xml"), "File2Data");
            File.WriteAllText(Path.Combine(this.tempDir, "System.File3.dat"), "File3Data");
            Directory.CreateDirectory(Path.Combine(this.tempDir, "SubFolder"));
            Directory.CreateDirectory(Path.Combine(this.tempDir, "SubFolder", "Sub2"));
            File.WriteAllText(Path.Combine(this.tempDir, "SubFolder", "Microsoft.File4.db"), "File4Data");
            File.WriteAllText(Path.Combine(this.tempDir, "SubFolder", "Sub2", "Taxi.File5.pdb"), "File5Data");
            this.varibles = new Dictionary<string, string>();
            this.varibles["TempPath"] = this.tempDir;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Directory.Delete(tempDir, true);
            this.varibles = null;
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
            purge.Execute(new ExecuteCommandArgs(msg => { }, this.varibles, buildFileCommand,null));
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
            purge.Execute(new ExecuteCommandArgs(msg => { }, this.varibles, buildFileCommand,null));
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
            purge.Execute(new ExecuteCommandArgs(msg => { }, this.varibles, buildFileCommand,null));
            Assert.IsTrue(File.Exists(Path.Combine(this.tempDir, "File1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(this.tempDir, "File2.xml")));
            Assert.IsTrue(File.Exists(Path.Combine(this.tempDir, "System.File3.dat")));
            Assert.IsFalse(File.Exists(Path.Combine(this.tempDir, "SubFolder", "Microsoft.File4.db")));
            Assert.IsFalse(File.Exists(Path.Combine(this.tempDir, "SubFolder", "Sub2", "Taxi.File5.pdb")));
            Assert.IsFalse(Directory.Exists(Path.Combine(this.tempDir, "SubFolder")));
        }
    }
}