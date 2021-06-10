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
        private string sampleProjectPathFileBadLengths;
        private string sampleProjectPathAssemblyBadLengths;
        private string sampleProjectPathNonIntegerFileVersionRevision;

        [TestInitialize]
        public void InitTest()
        {
            this.tempFolder = Path.Combine(Path.GetTempPath(), "VersionTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(this.tempFolder);
            this.sampleProjectPath = Path.Combine(this.tempFolder, "SampleProject.csproj");
            this.sampleProjectPathFileBadLengths = Path.Combine(this.tempFolder, "SampleProjectBadFileVersionLength.csproj");
            this.sampleProjectPathAssemblyBadLengths = Path.Combine(this.tempFolder, "SampleProjectBadAssemblyVersionLength.csproj");
            this.sampleProjectPathNonIntegerFileVersionRevision = Path.Combine(this.tempFolder, "SampleProjectNonIntegerFileVersionRevision.csproj");

            var sourceFileName = Path.Combine(Environment.CurrentDirectory, "SampleData", "SampleProject.csproj");
            File.Copy(sourceFileName, this.sampleProjectPath);

            sourceFileName = Path.Combine(Environment.CurrentDirectory, "SampleData", "SampleProjectBadFileVersionLength.csproj");
            File.Copy(sourceFileName, this.sampleProjectPathFileBadLengths);

            sourceFileName = Path.Combine(Environment.CurrentDirectory, "SampleData", "SampleProjectBadAssemblyVersionLength.csproj");
            File.Copy(sourceFileName, this.sampleProjectPathAssemblyBadLengths);

            sourceFileName = Path.Combine(Environment.CurrentDirectory, "SampleData", "SampleProjectNonIntegerFileVersionRevision.csproj");
            File.Copy(sourceFileName, this.sampleProjectPathNonIntegerFileVersionRevision);
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
        public void ValidateName()
        {
            var command = new VersionUpdaterCommand();
            Assert.AreEqual("updateversion", command.Name);
        }

        [TestMethod]
        public void NullArgs()
        {
            var command = new VersionUpdaterCommand();
            Assert.ThrowsException<ArgumentNullException>(() => command.Run(null));
        }

        [TestMethod]
        public void DefaultNoParameters()
        {
            var command = new VersionUpdaterCommand();
            var vars = new Dictionary<string, object>();
            var parameters = new Dictionary<string, object>();
            var cmdNode = new CommandData(CommandName, parameters);
            var args = new RunCommandArgs(null, vars, cmdNode, new BuildHelper());

            // run
            command.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            Assert.IsNotNull(args.Result.Error);
            Assert.AreSame(typeof(MissingParameterException), args.Result.Error.GetType());
            var mpe = args.Result.Error as MissingParameterException;
            Assert.AreEqual("ProjectFileName", mpe.ParameterName);
        }

        [TestMethod]
        public void ProjectFileLockedNoSharing()
        {
            var command = new VersionUpdaterCommand();
            var vars = new Dictionary<string, object>();
            var parameters = new Dictionary<string, object>();
            parameters["ProjectFileName"] = this.sampleProjectPath;
            var cmdNode = new CommandData(CommandName, parameters);
            var args = new RunCommandArgs(null, vars, cmdNode, new BuildHelper());

            // lock file
            using (var fs = new FileStream(this.sampleProjectPath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                // run
                command.Run(args);
            }

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            Assert.IsNotNull(args.Result.Error);
            Assert.AreSame(typeof(IOException), args.Result.Error.GetType());
        }

        [TestMethod]
        public void ProjectFileLockedReadOnlySharing()
        {
            var command = new VersionUpdaterCommand();
            var vars = new Dictionary<string, object>();
            var parameters = new Dictionary<string, object>();
            parameters["ProjectFileName"] = this.sampleProjectPath;
            var cmdNode = new CommandData(CommandName, parameters);
            var args = new RunCommandArgs(null, vars, cmdNode, new BuildHelper());

            // lock file
            using (var fs = new FileStream(this.sampleProjectPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // run
                command.Run(args);
            }

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            Assert.IsNotNull(args.Result.Error);
            Assert.AreSame(typeof(IOException), args.Result.Error.GetType());
        }

        [TestMethod]
        public void ProjectFileVersionBadFileLength()
        {
            var command = new VersionUpdaterCommand();
            var vars = new Dictionary<string, object>();
            var parameters = new Dictionary<string, object>();
            parameters["ProjectFileName"] = this.sampleProjectPathFileBadLengths;
            var cmdNode = new CommandData(CommandName, parameters);
            var args = new RunCommandArgs(null, vars, cmdNode, new BuildHelper());

            // run
            command.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            Assert.IsNotNull(args.Result.Error);
            Assert.AreSame(typeof(BuildException), args.Result.Error.GetType());
        }

        [TestMethod]
        public void ProjectFileVersionBadAssemblyLength()
        {
            var command = new VersionUpdaterCommand();
            var vars = new Dictionary<string, object>();
            var parameters = new Dictionary<string, object>();
            parameters["ProjectFileName"] = this.sampleProjectPathAssemblyBadLengths;
            var cmdNode = new CommandData(CommandName, parameters);
            var args = new RunCommandArgs(null, vars, cmdNode, new BuildHelper());

            // run
            command.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            Assert.IsNotNull(args.Result.Error);
            Assert.AreSame(typeof(BuildException), args.Result.Error.GetType());
        }

        [TestMethod]
        public void ProjectFileNonIntegerFileRevision()
        {
            var command = new VersionUpdaterCommand();
            var vars = new Dictionary<string, object>();
            var parameters = new Dictionary<string, object>();
            parameters["ProjectFileName"] = this.sampleProjectPathNonIntegerFileVersionRevision;
            var cmdNode = new CommandData(CommandName, parameters);
            var args = new RunCommandArgs(null, vars, cmdNode, new BuildHelper());

            // run
            command.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            Assert.IsNotNull(args.Result.Error);
            Assert.AreSame(typeof(BuildException), args.Result.Error.GetType());
        }

        [TestMethod]
        public void FileParameterIsTrue()
        {
            var date = DateTime.Now;
            var command = new VersionUpdaterCommand();
            var vars = new Dictionary<string, object>();
            var parameters = new Dictionary<string, object>();
            parameters["file"] = true;
            parameters["ProjectFileName"] = this.sampleProjectPath;


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
            var vars = new Dictionary<string, object>();
            var parameters = new Dictionary<string, object>();
            parameters["assembly"] = true;
            parameters["ProjectFileName"] = this.sampleProjectPath;

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
            var vars = new Dictionary<string, object>();
            var parameters = new Dictionary<string, object>();
            parameters["file"] = false;
            parameters["ProjectFileName"] = this.sampleProjectPath;

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
            var vars = new Dictionary<string, object>();
            var parameters = new Dictionary<string, object>();
            parameters["assembly"] = false;
            parameters["ProjectFileName"] = this.sampleProjectPath;

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