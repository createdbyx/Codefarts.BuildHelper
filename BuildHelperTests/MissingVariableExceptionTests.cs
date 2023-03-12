// <copyright file="MissingVariableExceptionTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using Codefarts.BuildHelper.Exceptions;

namespace BuildHelperTests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [TestCategory("MissingVariableException")]
    public class MissingVariableExceptionTests
    {
        [TestMethod]
        public void Ctor()
        {
            var ex = new MissingVariableException();
            Assert.IsNull(ex.VariableName);
        }

        [TestMethod]
        public void CtorWithParamName()
        {
            var ex = new MissingVariableException("param");
            Assert.AreEqual("param", ex.VariableName);
        }

        [TestMethod]
        public void CtorWithParamNameAndMessage()
        {
            var ex = new MissingVariableException("param", "msg");
            Assert.AreEqual("param", ex.VariableName);
            Assert.AreEqual("msg", ex.Message);
        }

        [TestMethod]
        public void CtorWithMessageAndInnerException()
        {
            var ex = new MissingVariableException("msg", new Exception());
            Assert.AreEqual("msg", ex.Message);
            Assert.IsNull(ex.VariableName);
            Assert.IsNotNull(ex.InnerException);
        }
    }
}