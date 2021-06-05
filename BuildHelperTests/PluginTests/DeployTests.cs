// <copyright file="DeployTests.cs" company="Codefarts">
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

    [TestClass]
    [TestCategory(nameof(DeployCommand))]
    public class DeployTests
    {
        private string tempFolder;

        [TestInitialize]
        public void InitTest()
        {
            this.tempFolder = Path.Combine(Path.GetTempPath(), "DeployTests_" + Guid.NewGuid().ToString("N"));
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
        public void MissingProjectDirVariable()
        {
            var deploy = new DeployCommand();
            var vars = new Dictionary<string, object>();
            // vars["ProjectDir"] = this.tempFolder;  <-- $(ProjectDir) is required
            vars["ConfigurationName"] = "DEBUG";
            vars["OutDir"] = "bin";

            var parameters = new Dictionary<string, object>();
            parameters["path"] = @"$(ProjectDir)\bin\$(ConfigurationName)\";
            parameters["clean"] = "true";
            parameters["message"] = "Test Message.";

            var cmdNode = new CommandData("deploy", parameters);
            var args = new RunCommandArgs(null, vars, cmdNode, new BuildHelper());

            Assert.ThrowsException<MissingVariableException>(() => deploy.Run(args));
        }

        [TestMethod]
        public void MissingOutputDirVariable()
        {
            var deploy = new DeployCommand();
            var vars = new Dictionary<string, object>();
            vars["ProjectDir"] = this.tempFolder;
            vars["ConfigurationName"] = "DEBUG";
            // vars["OutDir"] = "bin";   <-- $(OutDir) is required

            var parameters = new Dictionary<string, object>();
            parameters["path"] = @"$(ProjectDir)\bin\$(ConfigurationName)\";
            parameters["clean"] = "true";
            parameters["message"] = "Test Message.";

            var cmdNode = new CommandData("deploy", parameters);
            var args = new RunCommandArgs(null, vars, cmdNode, new BuildHelper());

            Assert.ThrowsException<MissingVariableException>(() => deploy.Run(args));
        }

        [TestMethod]
        public void MissingAllVariable()
        {
            var deploy = new DeployCommand();
            var vars = new Dictionary<string, object>();

            var parameters = new Dictionary<string, object>();
            parameters["path"] = @"$(ProjectDir)\bin\$(ConfigurationName)\";
            parameters["clean"] = "true";
            parameters["message"] = "Test Message.";

            var cmdNode = new CommandData("deploy", parameters);
            var args = new RunCommandArgs(null, vars, cmdNode, new BuildHelper());

            Assert.ThrowsException<MissingVariableException>(() => deploy.Run(args));
        }

        [TestMethod]
        public void NullVariablesDictionary()
        {
            var deploy = new DeployCommand();

            var parameters = new Dictionary<string, object>();
            parameters["path"] = @"$(ProjectDir)\bin\$(ConfigurationName)\";
            parameters["clean"] = "true";
            parameters["message"] = "Test Message.";

            var cmdNode = new CommandData("deploy", parameters);
            var args = new RunCommandArgs(null, null, cmdNode, new BuildHelper());

            Assert.ThrowsException<MissingVariableException>(() => deploy.Run(args));
        }

        [TestMethod]
        public void NullArguments()
        {
            var deploy = new DeployCommand();
            Assert.ThrowsException<ArgumentNullException>(() => deploy.Run(null));
        }

        [TestMethod]
        public void NoFiles()
        {
            var deploy = new DeployCommand();
            var vars = new Dictionary<string, object>();
            vars["ProjectDir"] = this.tempFolder;
            vars["ConfigurationName"] = "DEBUG";
            vars["OutDir"] = "Source";

            var parameters = new Dictionary<string, object>();
            parameters["path"] = @"$(ProjectDir)\bin\$(ConfigurationName)\";
            parameters["clean"] = "true";
            parameters["message"] = "Test Message.";

            //  <deploy path="$(SolutionDir)StockWatchWpfCore\bin\$(ConfigurationName)\$(ProjFolder)\Views\Settings\$(ProjectName)" clean="true" >
            var cmdNode = new CommandData("deploy", parameters);

            var args = new RunCommandArgs(null, vars, cmdNode, new BuildHelper());
            deploy.Run(args);

            Assert.IsFalse(File.Exists(Path.Combine(this.tempFolder, "bin", "DEBUG", "file1.txt")));
        }

        [TestMethod]
        public void DeployFiles()
        {
            TestHelpers.BuildFoldersAndFiles(this.tempFolder);

            var deploy = new DeployCommand();
            var vars = new Dictionary<string, object>();
            vars["ProjectDir"] = this.tempFolder;
            vars["ConfigurationName"] = "DEBUG";
            vars["OutDir"] = string.Empty;

            var parameters = new Dictionary<string, object>();
            parameters["path"] = @"$(ProjectDir)\DeployPath_$(ConfigurationName)\";
            parameters["clean"] = "true";
            parameters["message"] = "Test Message.";

            var cmdNode = new CommandData("deploy", parameters);

            var args = new RunCommandArgs(null, vars, cmdNode, new BuildHelper());

            var srcPath = parameters["path"].ToString().ReplaceVariableStrings(vars);
            Assert.IsFalse(Directory.Exists(srcPath));

            deploy.Run(args);

            var deployPath = srcPath;
            Assert.IsTrue(File.Exists(Path.Combine(deployPath, "File1.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(deployPath, "File2.xml")));
            Assert.IsTrue(File.Exists(Path.Combine(deployPath, "System.file3.dat")));
            Assert.IsTrue(File.Exists(Path.Combine(deployPath, "SubFolder", "Microsoft.file4.db")));
            Assert.IsTrue(File.Exists(Path.Combine(deployPath, "SubFolder", "Sub2", "Taxi.file5.pdb")));
            Assert.IsTrue(File.Exists(Path.Combine(deployPath, "Source", "file1.txt")));
        }
    }
}