// <copyright file="ConditionContains.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using Codefarts.BuildHelper;

namespace BuildHelperTests
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, TestCategory("Extension Methods")]
    public class ConditionContains
    {
        private XElement goodConditionEquality;
        private XElement goodConditionEqualityWithTrueIgnore;
        private XElement goodConditionEqualityWithFalseIgnore;
        private IDictionary<string, object> varibles;

        [TestInitialize]
        public void InitTest()
        {
            this.goodConditionEquality = new XElement("condition", new XAttribute("value1", "Test"), new XAttribute("operator", "="), new XAttribute("value2", "Test"));
            this.goodConditionEqualityWithTrueIgnore = new XElement("condition", new XAttribute("value1", "Test"), new XAttribute("operator", "="), new XAttribute("value2", "Test"), new XAttribute("ignorecase", true));
            this.goodConditionEqualityWithFalseIgnore = new XElement("condition", new XAttribute("value1", "Test"), new XAttribute("operator", "="), new XAttribute("value2", "Test"), new XAttribute("ignorecase", false));
            this.varibles = new Dictionary<string, object>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.goodConditionEquality = null;
            this.varibles = null;
        }

        [TestMethod]
        public void GoodButMissingValue2_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"Test\" operator=\"contains\" value2=\"\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void GoodButMissingValue1_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"contains\" value2=\"Test\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsFalse(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void GoodButValuesDiffer_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"Zest\" operator=\"contains\" value2=\"Test\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsFalse(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void SameValuesSameCasing_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"Test\" operator=\"contains\" value2=\"Test\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void SameValuesDiffernentCasing_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"test\" operator=\"contains\" value2=\"Test\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void SameValuesDiffernentCasing_IgnoreSetToFalse()
        {
            var item = XElement.Parse("<condition value1=\"test\" operator=\"contains\" value2=\"Test\" ignorecase=\"false\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsFalse(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void SameValuesDiffernentCasing_IgnoreSetToTrue()
        {
            var item = XElement.Parse("<condition value1=\"test\" operator=\"contains\" value2=\"Test\" ignorecase=\"true\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void SameValuesDiffernentCasing_IgnoreSetToTRUE()
        {
            var item = XElement.Parse("<condition value1=\"test\" operator=\"contains\" value2=\"Test\" ignorecase=\"TRUE\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void GoodButMissingValues_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"contains\" value2=\"\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void GoodButMissingValues_IgnoreNotSpecified()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"contains\" value2=\"\" ignorecase=\"\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void VariblesArgNull()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"contains\" value2=\"\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.varibles));
        }
    }
}