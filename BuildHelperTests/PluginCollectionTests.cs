// <copyright file="PluginCollectionTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace BuildHelperTests
{
    using System.Collections.Generic;
    using BuildHelperTests.Mocks;
    using Codefarts.BuildHelper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, TestCategory("PluginCollection Tests")]
    public class PluginCollectionTests
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
        public void CtorNoArgs()
        {
            var items = new PluginCollection();
            Assert.AreEqual(0, items.Count);
        }

        [TestMethod]
        public void CtorWithArrayArgs()
        {
            var existing = new List<ICommandPlugin>();
            existing.Add(new MockCommandPlugin("one"));
            existing.Add(new MockCommandPlugin("two"));
            existing.Add(new MockCommandPlugin("three"));
            var items = new PluginCollection(existing.ToArray());
            Assert.AreEqual(3, items.Count);
        }

        [TestMethod]
        public void CtorWithListArgs()
        {
            var existing = new List<ICommandPlugin>();
            existing.Add(new MockCommandPlugin("one"));
            existing.Add(new MockCommandPlugin("two"));
            existing.Add(new MockCommandPlugin("three"));
            var items = new PluginCollection(existing);
            Assert.AreEqual(3, items.Count);
        }
    }
}