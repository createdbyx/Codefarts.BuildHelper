// <copyright file="MissingParameterExceptionTests.cs" company="Codefarts">
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
    [TestCategory("MissingParameterException")]
    public class MissingParameterExceptionTests
    {
        [TestMethod]
        public void Ctor()
        {
            var ex = new MissingParameterException();
            Assert.IsNull(ex.ParameterName);
        }

        [TestMethod]
        public void CtorWithParamName()
        {
            var ex = new MissingParameterException("param");
            Assert.AreEqual("param", ex.ParameterName);
        }

        [TestMethod]
        public void CtorWithParamNameAndMessage()
        {
            var ex = new MissingParameterException("param", "msg");
            Assert.AreEqual("param", ex.ParameterName);
            Assert.AreEqual("msg", ex.Message);
        }

        [TestMethod]
        public void CtorWithMessageAndInnerException()
        {
            var ex = new MissingParameterException("msg", new Exception());
            Assert.AreEqual("msg", ex.Message);
            Assert.IsNull(ex.ParameterName);
            Assert.IsNotNull(ex.InnerException);
        }
    }
}