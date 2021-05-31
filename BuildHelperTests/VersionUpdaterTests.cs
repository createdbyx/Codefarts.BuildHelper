// <copyright file="VersionUpdaterTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace BuildHelperTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using AutoVersionUpdater;
    using Codefarts.BuildHelper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, TestCategory("VersionUpdater")]
    public class VersionUpdaterTests
    {
        private const string CommandName = "updateversion";
        private string tempFolder;
        private string sampleProjectPath;

        [TestInitialize]
        public void InitTest()
        {
            this.tempFolder = Path.Combine(Path.GetTempPath(), "VersionTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(this.tempFolder);
            this.sampleProjectPath = Path.Combine(this.tempFolder, "SampleProject.csproj");
            var sourceFileName = Path.Combine(Environment.CurrentDirectory, "SampleData", "SampleProject.csproj");
            File.Copy(sourceFileName, this.sampleProjectPath);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (Directory.Exists(this.tempFolder))
            {
                Directory.Delete(this.tempFolder, true);
            }
        }

        [TestMethod]
        public void DefaultNoParameters()
        {
            var date = DateTime.Now;
            var command = new VersionUpdaterCommand();
            var vars = new Dictionary<string, string>();
            vars["ProjectFileName"] = this.sampleProjectPath;

            var parameters = new Dictionary<string, object>();

            var cmdNode = new CommandData(CommandName, parameters);
            var args = new RunCommandArgs(null, vars, cmdNode, new BuildHelper());

            // run
            command.Run(args);

            // validate data
            var fileData = File.ReadAllText(this.sampleProjectPath);

            Assert.IsTrue(fileData.Contains($"<AssemblyVersion>{date.Year}.{date.Month}.{date.Day}.1</AssemblyVersion>"));
            Assert.IsTrue(fileData.Contains($"<FileVersion>{date.Year}.{date.Month}.{date.Day}.1</FileVersion>"));
        }

        [TestMethod]
        public void FileParameterIsTrue()
        {
            var date = DateTime.Now;
            var command = new VersionUpdaterCommand();
            var vars = new Dictionary<string, string>();
            vars["ProjectFileName"] = this.sampleProjectPath;

            var parameters = new Dictionary<string, object>();
            parameters["file"] = true;

            var cmdNode = new CommandData(CommandName, parameters);
            var args = new RunCommandArgs(null, vars, cmdNode, new BuildHelper());

            // run
            command.Run(args);

            // validate data
            var fileData = File.ReadAllText(this.sampleProjectPath);

            Assert.IsTrue(fileData.Contains($"<AssemblyVersion>{date.Year}.{date.Month}.{date.Day}.1</AssemblyVersion>"));
            Assert.IsTrue(fileData.Contains($"<FileVersion>{date.Year}.{date.Month}.{date.Day}.1</FileVersion>"));
        }

        [TestMethod]
        public void AssemblyParameterIsTrue()
        {
            var date = DateTime.Now;
            var command = new VersionUpdaterCommand();
            var vars = new Dictionary<string, string>();
            vars["ProjectFileName"] = this.sampleProjectPath;

            var parameters = new Dictionary<string, object>();
            parameters["assembly"] = true;

            var cmdNode = new CommandData(CommandName, parameters);
            var args = new RunCommandArgs(null, vars, cmdNode, new BuildHelper());

            // run
            command.Run(args);

            // validate data
            var fileData = File.ReadAllText(this.sampleProjectPath);

            Assert.IsTrue(fileData.Contains($"<AssemblyVersion>{date.Year}.{date.Month}.{date.Day}.1</AssemblyVersion>"));
            Assert.IsTrue(fileData.Contains($"<FileVersion>{date.Year}.{date.Month}.{date.Day}.1</FileVersion>"));
        }

        [TestMethod]
        public void FileParameterIsFalse()
        {
            var date = DateTime.Now;
            var command = new VersionUpdaterCommand();
            var vars = new Dictionary<string, string>();
            vars["ProjectFileName"] = this.sampleProjectPath;

            var parameters = new Dictionary<string, object>();
            parameters["file"] = false;

            var cmdNode = new CommandData(CommandName, parameters);
            var args = new RunCommandArgs(null, vars, cmdNode, new BuildHelper());

            // run
            command.Run(args);

            // validate data
            var fileData = File.ReadAllText(this.sampleProjectPath);

            Assert.IsTrue(fileData.Contains($"<AssemblyVersion>{date.Year}.{date.Month}.{date.Day}.1</AssemblyVersion>"));
            Assert.IsTrue(fileData.Contains($"<FileVersion>2021.1.25.0</FileVersion>"));
        }

        [TestMethod]
        public void AssemblyParameterIsFalse()
        {
            var date = DateTime.Now;
            var command = new VersionUpdaterCommand();
            var vars = new Dictionary<string, string>();
            vars["ProjectFileName"] = this.sampleProjectPath;

            var parameters = new Dictionary<string, object>();
            parameters["assembly"] = false;

            var cmdNode = new CommandData(CommandName, parameters);
            var args = new RunCommandArgs(null, vars, cmdNode, new BuildHelper());

            // run
            command.Run(args);

            // validate data
            var fileData = File.ReadAllText(this.sampleProjectPath);

            Assert.IsTrue(fileData.Contains($"<AssemblyVersion>2021.1.25.0</AssemblyVersion>"));
            Assert.IsTrue(fileData.Contains($"<FileVersion>{date.Year}.{date.Month}.{date.Day}.1</FileVersion>"));
        }
    }
}