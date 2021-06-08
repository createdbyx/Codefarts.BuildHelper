// <copyright file="VariableTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace BuildHelperTests
{
    using System;
    using System.Collections.Generic;
    using Codefarts.BuildHelper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, TestCategory("Variable Tests")]
    public class VariableTests
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
        public void NullArgs()
        {
            var command = new AssignVariableCommand();
            Assert.ThrowsException<ArgumentNullException>(() => command.Run(null));
        }

        [TestMethod]
        public void ValidateName()
        {
            var command = new AssignVariableCommand();
            Assert.AreEqual("variable", command.Name);
        }

        [TestMethod]
        public void SetVariable()
        {
            var command = new AssignVariableCommand();
            var data = new CommandData();
            data.Parameters["name"] = "name";
            data.Parameters["value"] = "value";
            var variables = new Dictionary<string, object>();
            var args = new RunCommandArgs(null, variables, data, new BuildHelper());
            command.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Sucessful, args.Result.Status);
            Assert.IsTrue(variables.ContainsKey("name"));
            Assert.AreEqual("value", variables["name"]);
        }

        [TestMethod]
        public void SetVariableMissingName()
        {
            var command = new AssignVariableCommand();
            var data = new CommandData();
            data.Parameters["value"] = "value";
            var variables = new Dictionary<string, object>();
            var args = new RunCommandArgs(null, variables, data, new BuildHelper());
            command.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            var resultError = args.Result.Error;
            Assert.AreSame(typeof(MissingParameterException), resultError.GetType());

            var mpe = resultError as MissingParameterException;
            Assert.AreEqual("name", mpe.ParameterName);
        }

        [TestMethod]
        public void SetVariableMissingValue()
        {
            var command = new AssignVariableCommand();
            var data = new CommandData();
            data.Parameters["name"] = "name";
            var variables = new Dictionary<string, object>();
            var args = new RunCommandArgs(null, variables, data, new BuildHelper());
            command.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            var resultError = args.Result.Error;
            Assert.AreSame(typeof(MissingParameterException), resultError.GetType());

            var mpe = resultError as MissingParameterException;
            Assert.AreEqual("value", mpe.ParameterName);
        }

        [TestMethod]
        public void ReplaceVariable()
        {
            var command = new AssignVariableCommand();
            var data = new CommandData();
            data.Parameters["name"] = "name";
            data.Parameters["value"] = "newValue";
            var variables = new Dictionary<string, object>();
            variables["name"] = "oldValue";
            var args = new RunCommandArgs(null, variables, data, new BuildHelper());
            command.Run(args);

            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Sucessful, args.Result.Status);
            Assert.IsTrue(variables.ContainsKey("name"));
            Assert.AreEqual("newValue", variables["name"]);
        }
    }
}