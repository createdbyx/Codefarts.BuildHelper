// <copyright file="NamedParameterTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace BuildHelperTests
{
    using System;
    using Codefarts.BuildHelper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, TestCategory("Named Parameter")]
    public class NamedParameterTests
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
            Assert.ThrowsException<ArgumentNullException>(() => new NamedParameter(null, null));
        }

        [TestMethod]
        public void ValidNameNullType()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new NamedParameter("name", null));
        }

        [TestMethod]
        public void NullNameValidType()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new NamedParameter(null, typeof(float)));
        }

        [TestMethod]
        public void NameWasSet()
        {
            var type = typeof(float);
            var att = new NamedParameter("name", type);
            Assert.AreEqual("name", att.Name);
            Assert.AreSame(type, att.Type);
            Assert.AreEqual(false, att.Required);
            Assert.AreEqual(null, att.Description);
        }

        [TestMethod]
        public void RequiredWasSet()
        {
            var type = typeof(float);
            var att = new NamedParameter("name", type, true);
            Assert.AreEqual("name", att.Name);
            Assert.AreSame(type, att.Type);
            Assert.AreEqual(true, att.Required);
            Assert.AreEqual(null, att.Description);
        }

        [TestMethod]
        public void DescriptionWasSet()
        {
            var type = typeof(float);
            var att = new NamedParameter("name", type, description: "desc");
            Assert.AreEqual("name", att.Name);
            Assert.AreSame(type, att.Type);
            Assert.AreEqual(false, att.Required);
            Assert.AreEqual("desc", att.Description);
        }


        [TestMethod]
        public void AllPropertiesSet()
        {
            var type = typeof(float);
            var att = new NamedParameter("name", type, true, "desc");
            Assert.AreEqual("name", att.Name);
            Assert.AreSame(type, att.Type);
            Assert.AreEqual(true, att.Required);
            Assert.AreEqual("desc", att.Description);
        }
    }
}