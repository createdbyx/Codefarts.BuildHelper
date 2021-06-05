// <copyright file="ConditionCommandTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace BuildHelperTests
{
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Codefarts.BuildHelper;
    using ConditionCommand;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, TestCategory("ConditionalPlugin")]
    public class ConditionCommandTests
    {
        private IDictionary<string, object> variables;
        private ICommandPlugin plugin;

        [TestInitialize]
        public void InitTest()
        {
            this.plugin = new ConditionCommand();
            this.variables = new Dictionary<string, object>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.plugin = null;
            this.variables = null;
        }

        [TestMethod]
        [TestCategory("ConditionalPlugin-Contains")]
        public void GoodButMissingValue2_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"Test\" operator=\"contains\" value2=\"\" />");
            var helper = new BuildHelper();
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(null, this.variables, data, helper);

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
            var helper = new BuildHelper();
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(null, this.variables, data, helper);

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
            var helper = new BuildHelper();
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(null, this.variables, data, helper);

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
            var helper = new BuildHelper();
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(null, this.variables, data, helper);

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
            var helper = new BuildHelper();
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(null, this.variables, data, helper);

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
            var helper = new BuildHelper();
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(null, this.variables, data, helper);

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
            var helper = new BuildHelper();
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(null, this.variables, data, helper);

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
            var helper = new BuildHelper();
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(null, this.variables, data, helper);

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
            var helper = new BuildHelper();
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(null, this.variables, data, helper);

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
            var helper = new BuildHelper();
            var data = TestHelpers.BuildCommandNode(item, null);
            var args = new RunCommandArgs(null, this.variables, data, helper);

            this.plugin.Run(args);
            var result = args.Result;
            Assert.IsNotNull(result);
            Assert.AreEqual(RunStatus.Sucessful, result.Status);
            Assert.IsTrue(result.GetReturnValue<bool>());
        }
    }
}