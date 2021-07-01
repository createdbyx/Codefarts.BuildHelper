// <copyright file="ConditionCommandTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace BuildHelperTests
{
    using System;
    using System.Xml.Linq;
    using Codefarts.BuildHelper;
    using ConditionCommand;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, TestCategory("ConditionalPlugin")]
    public class ConditionCommandTests
    {
        private VariablesDictionary variables;
        private ICommandPlugin plugin;

        [TestInitialize]
        public void InitTest()
        {
            this.plugin = new ConditionCommand();
            this.variables = new VariablesDictionary();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.plugin = null;
            this.variables = null;
        }

        [TestMethod]
        public void ValidateCommandName()
        {
            Assert.AreEqual("condition", this.plugin.Name);
        }

        [TestMethod]
        public void RunWithNullArgs()
        {
            Assert.ThrowsException<ArgumentNullException>(() => this.plugin.Run(null));
        }

        [TestMethod]
        public void MissingValue2()
        {
            var item = XElement.Parse("<condition value1=\"Test\" operator=\"contains\" />");
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, data);

            this.plugin.Run(args);
            var result = args.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(RunStatus.Errored, result.Status);
            Assert.IsNotNull(result.Error);

            var mpe = result.Error as MissingParameterException;
            Assert.IsNotNull(mpe);
            Assert.AreEqual("value2", mpe.ParameterName);
        }

        [TestMethod]
        public void MissingValue1()
        {
            var item = XElement.Parse("<condition value2=\"Test\" operator=\"contains\" />");
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, data);

            this.plugin.Run(args);
            var result = args.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(RunStatus.Errored, result.Status);
            Assert.IsNotNull(result.Error);

            var mpe = result.Error as MissingParameterException;
            Assert.IsNotNull(mpe);
            Assert.AreEqual("value1", mpe.ParameterName);
        }

        [TestMethod]
        public void MissingOperator()
        {
            var item = XElement.Parse("<condition value1=\"Test\" value2=\"Test\"   />");
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, data);

            this.plugin.Run(args);
            var result = args.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(RunStatus.Errored, result.Status);
            Assert.IsNotNull(result.Error);

            var mpe = result.Error as MissingParameterException;
            Assert.IsNotNull(mpe);
            Assert.AreEqual("operator", mpe.ParameterName);
        }

        [TestMethod]
        public void Value1IsObject()
        {
            var item = XElement.Parse("<condition value1=\"Test\" operator=\"invalid\" value2=\"Test\"   />");
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, data);
            args.Command.Parameters["value1"] = new CommandData("TestObj1");

            this.plugin.Run(args);
            var result = args.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(RunStatus.Errored, result.Status);
            Assert.IsNotNull(result.Error);

            var nse = result.Error as NotSupportedException;
            Assert.IsNotNull(nse);
        }

        [TestMethod]
        public void InvalidOperator()
        {
            var item = XElement.Parse("<condition value1=\"Test\" operator=\"invalid\" value2=\"Test\"   />");
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, data);

            this.plugin.Run(args);
            var result = args.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(RunStatus.Errored, result.Status);
            Assert.IsNotNull(result.Error);

            var nse = result.Error as NotSupportedException;
            Assert.IsNotNull(nse);
        }

        [TestMethod]
        [TestCategory("ConditionalPlugin-Contains")]
        public void GoodButMissingValue2_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"Test\" operator=\"contains\" value2=\"\" />");
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, data);

            this.plugin.Run(args);
            var result = args.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(RunStatus.Sucessful, result.Status);
            Assert.IsTrue(result.GetReturnValue<bool>());
        }

        [TestMethod]
        [TestCategory("ConditionalPlugin-Contains")]
        public void GoodButMissingValue1_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"contains\" value2=\"Test\" />");
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, data);

            this.plugin.Run(args);
            var result = args.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(RunStatus.Sucessful, result.Status);
            Assert.IsFalse(result.GetReturnValue<bool>());
        }

        [TestMethod]
        [TestCategory("ConditionalPlugin-Contains")]
        public void GoodButValuesDiffer_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"Zest\" operator=\"contains\" value2=\"Test\" />");
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, data);

            this.plugin.Run(args);
            var result = args.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(RunStatus.Sucessful, result.Status);
            Assert.IsFalse(result.GetReturnValue<bool>());
        }

        [TestMethod]
        [TestCategory("ConditionalPlugin-Contains")]
        public void SameValuesSameCasing_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"Test\" operator=\"contains\" value2=\"Test\" />");
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, data);

            this.plugin.Run(args);
            var result = args.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(RunStatus.Sucessful, result.Status);
            Assert.IsTrue(result.GetReturnValue<bool>());
        }

        [TestMethod]
        [TestCategory("ConditionalPlugin-Contains")]
        public void SameValuesDiffernentCasing_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"test\" operator=\"contains\" value2=\"Test\" />");
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, data);

            this.plugin.Run(args);
            var result = args.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(RunStatus.Sucessful, result.Status);
            Assert.IsFalse(result.GetReturnValue<bool>());
        }

        [TestMethod]
        [TestCategory("ConditionalPlugin-Contains")]
        public void SameValuesDiffernentCasing_IgnoreSetToFalse()
        {
            var item = XElement.Parse("<condition value1=\"test\" operator=\"contains\" value2=\"Test\" ignorecase=\"false\" />");
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, data);

            this.plugin.Run(args);
            var result = args.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(RunStatus.Sucessful, result.Status);
            Assert.IsFalse(result.GetReturnValue<bool>());
        }

        [TestMethod]
        [TestCategory("ConditionalPlugin-Contains")]
        public void SameValuesDiffernentCasing_IgnoreSetToTrue()
        {
            var item = XElement.Parse("<condition value1=\"test\" operator=\"contains\" value2=\"Test\" ignorecase=\"true\" />");
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, data);

            this.plugin.Run(args);
            var result = args.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(RunStatus.Sucessful, result.Status);
            Assert.IsTrue(result.GetReturnValue<bool>());
        }

        [TestMethod]
        [TestCategory("ConditionalPlugin-Contains")]
        public void SameValuesDiffernentCasing_IgnoreSetToTRUE()
        {
            var item = XElement.Parse("<condition value1=\"test\" operator=\"contains\" value2=\"Test\" ignorecase=\"TRUE\" />");
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, data);

            this.plugin.Run(args);
            var result = args.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(RunStatus.Sucessful, result.Status);
            Assert.IsTrue(result.GetReturnValue<bool>());
        }

        [TestMethod]
        [TestCategory("ConditionalPlugin-Contains")]
        public void GoodButMissingValues_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"contains\" value2=\"\" />");
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, data);

            this.plugin.Run(args);
            var result = args.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(RunStatus.Sucessful, result.Status);
            Assert.IsTrue(result.GetReturnValue<bool>());
        }

        [TestMethod]
        [TestCategory("ConditionalPlugin-Contains")]
        public void GoodButMissingValues_IgnoreNotSpecified()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"contains\" value2=\"\" ignorecase=\"\" />");
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(this.variables, data);

            this.plugin.Run(args);
            var result = args.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(RunStatus.Sucessful, result.Status);
            Assert.IsTrue(result.GetReturnValue<bool>());
        }
    }
}