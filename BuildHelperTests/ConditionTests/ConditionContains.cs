// <copyright file="ConditionContains.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace BuildHelperTests
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Codefarts.BuildHelper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, TestCategory("Extension Methods")]
    public class ConditionContains
    {
        private IDictionary<string, object> variables;

        [TestInitialize]
        public void InitTest()
        {
            this.variables = new Dictionary<string, object>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.variables = null;
        }

        [TestMethod]
        public void GoodButMissingValue2_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"Test\" operator=\"contains\" value2=\"\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.variables));
        }

        [TestMethod]
        public void GoodButMissingValue1_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"contains\" value2=\"Test\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsFalse(node.SatifiesCondition(this.variables));
        }

        [TestMethod]
        public void GoodButValuesDiffer_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"Zest\" operator=\"contains\" value2=\"Test\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsFalse(node.SatifiesCondition(this.variables));
        }
        
        [TestMethod]
        public void GoodButValuesDiffer_SubPathString()
        {
            var item = XElement.Parse("<condition value1=\"c:\\SomePath\\SubPath.more\\file.txt\" operator=\"contains\" value2=\"path.\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.variables));
        }

        [TestMethod]
        public void SameValuesSameCasing_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"Test\" operator=\"contains\" value2=\"Test\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.variables));
        }

        [TestMethod]
        public void SameValuesDiffernentCasing_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"test\" operator=\"contains\" value2=\"Test\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.variables));
        }

        [TestMethod]
        public void SameValuesDiffernentCasing_IgnoreSetToFalse()
        {
            var item = XElement.Parse("<condition value1=\"test\" operator=\"contains\" value2=\"Test\" ignorecase=\"false\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsFalse(node.SatifiesCondition(this.variables));
        }

        [TestMethod]
        public void SameValuesDiffernentCasing_IgnoreSetToTrue()
        {
            var item = XElement.Parse("<condition value1=\"test\" operator=\"contains\" value2=\"Test\" ignorecase=\"true\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.variables));
        }

        [TestMethod]
        public void SameValuesDiffernentCasing_IgnoreSetToTRUE()
        {
            var item = XElement.Parse("<condition value1=\"test\" operator=\"contains\" value2=\"Test\" ignorecase=\"TRUE\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.variables));
        }

        [TestMethod]
        public void GoodButMissingValues_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"contains\" value2=\"\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.variables));
        }

        [TestMethod]
        public void GoodButMissingValues_IgnoreNotSpecified()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"contains\" value2=\"\" ignorecase=\"\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => node.SatifiesCondition(this.variables));
        }

        [TestMethod]
        public void VariblesArgNull()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"contains\" value2=\"\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.variables));
        }
    }
}