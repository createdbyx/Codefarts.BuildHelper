// <copyright file="ConditionEquality.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System;

namespace BuildHelperTests
{
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Codefarts.BuildHelper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, TestCategory("Extension Methods")]
    public class ConditionEquality
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
        public void GoodButValues2NotPresent_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"Test\" operator=\"=\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void GoodButValues1NotPresent_MissingIgnore()
        {
            var item = XElement.Parse("<condition operator=\"=\" value2=\"Test\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void GoodButMissingValues2_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"Test\" operator=\"=\" value2=\"\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsFalse(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void GoodButMissingValues1_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"=\" value2=\"Test\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsFalse(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void GoodButValuesDiffer_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"Zest\" operator=\"=\" value2=\"Test\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsFalse(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void SameValuesSameCasing_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"Test\" operator=\"=\" value2=\"Test\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void SameValuesDiffernentCasing_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"test\" operator=\"=\" value2=\"Test\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void SameValuesDiffernentCasing_IgnoreSetToFalse()
        {
            var item = XElement.Parse("<condition value1=\"test\" operator=\"=\" value2=\"Test\" ignorecase=\"false\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsFalse(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void SameValuesDiffernentCasing_IgnoreSetToTrue()
        {
            var item = XElement.Parse("<condition value1=\"test\" operator=\"=\" value2=\"Test\" ignorecase=\"true\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void SameValuesDiffernentCasing_IgnoreSetToTRUE()
        {
            var item = XElement.Parse("<condition value1=\"test\" operator=\"=\" value2=\"Test\" ignorecase=\"TRUE\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void GoodButMissingValues_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"=\" value2=\"\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void GoodButMissingValues_IgnoreNotSpecified()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"=\" value2=\"\" ignorecase=\"\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void GoodButNoOperator()
        {
            var item = XElement.Parse("<condition value1=\"\" value2=\"\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.ThrowsException<ArgumentNullException>(() => node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void GoodButMissingOperatorValue()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"\" value2=\"\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void GoodButMissingValues_BadIgnoreValueSpecified()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"=\" value2=\"\" ignorecase=\"Bad\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void NullArgs()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ExtensionMethods.SatifiesCondition(null, null));
        }

        [TestMethod]
        public void VariblesArgNull()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"=\" value2=\"\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(null));
        }

        [TestMethod]
        public void ElementArgNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ExtensionMethods.SatifiesCondition(null, this.varibles));
        }

        [TestMethod]
        public void ElementNameNotCondition()
        {
            var item = XElement.Parse("<badcondition value1=\"\" operator=\"=\" value2=\"\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.ThrowsException<ArgumentException>(() => node.SatifiesCondition(this.varibles));
        }
    }
}