// <copyright file="BuildExceptionTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace BuildHelperTests
{
    using System;
    using Codefarts.BuildHelper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [TestCategory("MissingVariableException")]
    public class BuildExceptionTests
    {
        [TestMethod]
        public void Ctor()
        {
            var ex = new BuildException();
            Assert.IsTrue(string.IsNullOrWhiteSpace(ex.Message));
        }

        [TestMethod]
        public void CtorWithMessage()
        {
            var ex = new BuildException("msg");
            Assert.AreEqual("msg", ex.Message);
        }

        [TestMethod]
        public void CtorWithMessageAndInnerException()
        {
            var ex = new BuildException("msg", new Exception());
            Assert.AreEqual("msg", ex.Message);
            Assert.IsNotNull(ex.InnerException);
        }
    }
}