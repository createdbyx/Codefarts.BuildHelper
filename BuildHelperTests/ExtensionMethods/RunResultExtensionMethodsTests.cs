// <copyright file="RunResultExtensionMethodsTests.cs" company="Codefarts">
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
    [TestCategory("Extension Methods - RunResult")]
    public class RunResultExtensionMethodsTests
    {
        [TestMethod]
        public void GetReturnValueThatDoesNotImplementIConvertableWithoutDefaultValue()
        {
            Assert.ThrowsException<InvalidCastException>(() =>
            {
                var result = new RunResult(new VariablesDictionary());
                var value = RunResultExtensionMethods.GetReturnValue<bool>(result);
                Assert.Fail($"Should have thrown {nameof(InvalidCastException)}.");
            });
        }

        [TestMethod]
        public void GetReturnValueThatDoesNotImplementIConvertableAndADefaultValue()
        {
            var result = new RunResult(new VariablesDictionary());
            var value = RunResultExtensionMethods.GetReturnValue<bool>(result, true);
            Assert.IsTrue(value);
        }


        [TestMethod]
        public void GetReturnValueWithNullArgsNoDefaultValue()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                RunResult result = null;
                RunResultExtensionMethods.GetReturnValue<bool>(result);
            });
        }

        [TestMethod]
        public void GetReturnValueWithNullArgsWithDefaultValue()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                RunResult result = null;
                RunResultExtensionMethods.GetReturnValue(result, "default");
            });
        }

        [TestMethod]
        public void GetNonBooleanReturnValueStringAsBooleanWithNoDefaultValue()
        {
            var result = RunResult.Sucessful("test");
            Assert.ThrowsException<FormatException>(() => { RunResultExtensionMethods.GetReturnValue<bool>(result); });
        }

        [TestMethod]
        public void GetReturnValueWithValidArgsBadCastingNoDefaultValue()
        {
            var result = RunResult.Sucessful("true");
            var value = RunResultExtensionMethods.GetReturnValue<bool>(result);
            Assert.IsTrue(value);
        }

        [TestMethod]
        public void GetReturnValueWithValidArgsCastingWithDefaultValue()
        {
            var result = RunResult.Sucessful(true);
            var value = RunResultExtensionMethods.GetReturnValue(result, "default");
            Assert.AreSame(typeof(string), value.GetType());
            Assert.AreEqual("True", value);
        }
    }
}