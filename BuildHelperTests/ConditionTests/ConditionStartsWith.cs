// <copyright file="ConditionStartsWith.cs" company="Codefarts">
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

    [TestClass, TestCategory("Extension Methods - CommandData.SatifiesCondition")]
    public class ConditionStartsWith
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
        public void GoodButValue1IsPartialMatch_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"te\" operator=\"startswith\" value2=\"Test\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsFalse(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void GoodButValueIsPartialMatch_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"Test\" operator=\"startswith\" value2=\"te\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void GoodButMissingValues2_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"Test\" operator=\"startswith\" value2=\"\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void GoodButMissingValues1_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"startswith\" value2=\"Test\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsFalse(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void GoodButValuesDiffer_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"Zest\" operator=\"startswith\" value2=\"Test\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsFalse(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void SameValuesSameCasing_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"Test\" operator=\"startswith\" value2=\"Test\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void SameValuesDiffernentCasing_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"test\" operator=\"startswith\" value2=\"Test\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void SameValuesDiffernentCasing_IgnoreSetToFalse()
        {
            var item = XElement.Parse("<condition value1=\"test\" operator=\"startswith\" value2=\"Test\" ignorecase=\"false\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsFalse(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void SameValuesDiffernentCasing_IgnoreSetToTrue()
        {
            var item = XElement.Parse("<condition value1=\"test\" operator=\"startswith\" value2=\"Test\" ignorecase=\"true\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void SameValuesDiffernentCasing_IgnoreSetToTRUE()
        {
            var item = XElement.Parse("<condition value1=\"test\" operator=\"startswith\" value2=\"Test\" ignorecase=\"TRUE\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void GoodButMissingValues_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"startswith\" value2=\"\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void GoodButMissingValues_MixedCaseOperator_MissingIgnore()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"StartsWith\" value2=\"\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void GoodButMissingValues_IgnoreNotSpecified()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"startswith\" value2=\"\" ignorecase=\"\" />");
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
            var item = XElement.Parse("<condition value1=\"\" operator=\"startswith\" value2=\"\" ignorecase=\"Bad\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => node.SatifiesCondition(this.varibles));
        }

        [TestMethod]
        public void VariblesArgNull()
        {
            var item = XElement.Parse("<condition value1=\"\" operator=\"startswith\" value2=\"\" />");
            var node = TestHelpers.BuildCommandNode(item, null);
            Assert.IsTrue(node.SatifiesCondition(null));
        }
    }
}