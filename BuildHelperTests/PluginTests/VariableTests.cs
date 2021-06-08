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
        public void AssignVariableNullArgs()
        {
            var command = new AssignVariableCommand();
            Assert.ThrowsException<ArgumentNullException>(() => command.Run(null));
        }

        [TestMethod]
        public void RemoveVariableNullArgs()
        {
            var command = new RemoveVariableCommand();
            Assert.ThrowsException<ArgumentNullException>(() => command.Run(null));
        }

        [TestMethod]
        public void AssignVariableValidateName()
        {
            var command = new AssignVariableCommand();
            Assert.AreEqual("variable", command.Name);
        }

        [TestMethod]
        public void RemoveVariableValidateName()
        {
            var command = new RemoveVariableCommand();
            Assert.AreEqual("removevariable", command.Name);
        }

        [TestMethod]
        public void RemoveExistingSetVariable()
        {
            var variables = this.SetVariable();
            var command = new RemoveVariableCommand();
            var data = new CommandData(command.Name);
            data.Parameters["name"] = "name";
            var args = new RunCommandArgs(null, variables, data, new BuildHelper());

            command.Run(args);
            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Sucessful, args.Result.Status);
            Assert.IsTrue(args.Result.GetReturnValue<bool>());
            Assert.IsFalse(variables.ContainsKey("name"));
        }

        [TestMethod]
        public void RemoveVariableMissingNameParameter()
        {
            var variables = this.SetVariable();
            var command = new RemoveVariableCommand();
            var data = new CommandData(command.Name);
            var args = new RunCommandArgs(null, variables, data, new BuildHelper());

            command.Run(args);
            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
            Assert.AreSame(typeof(MissingParameterException), args.Result.Error.GetType());

            var mpe = args.Result.Error as MissingParameterException;
            Assert.AreEqual("name", mpe.ParameterName);
        }

        [TestMethod]
        public void RemoveNonExistingSetVariable()
        {
            var variables = this.SetVariable();
            var command = new RemoveVariableCommand();
            var data = new CommandData(command.Name);
            data.Parameters["name"] = "notExisting";
            var args = new RunCommandArgs(null, variables, data, new BuildHelper());

            command.Run(args);
            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Sucessful, args.Result.Status);
            Assert.IsFalse(args.Result.GetReturnValue<bool>());
            Assert.IsTrue(variables.ContainsKey("name"));
            Assert.AreEqual(1, variables.Count);
        }

        [TestMethod]
        public IDictionary<string, object> SetVariable()
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

            return variables;
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