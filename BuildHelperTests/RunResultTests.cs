// <copyright file="RunResultTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System;

namespace BuildHelperTests
{
    using Codefarts.BuildHelper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [TestCategory(nameof(RunResultTests))]
    public class RunResultTests
    {
        [TestMethod]
        public void SucessfulHelper()
        {
            var result = RunResult.Sucessful();
            Assert.IsNotNull(result);
            Assert.AreEqual(RunStatus.Sucessful, result.Status);
            Assert.IsNull(result.Error);
            Assert.IsNull(result.ReturnValue);
        }

        [TestMethod]
        public void SucessfulHelperWithReturnValue()
        {
            var result = RunResult.Sucessful("test");
            Assert.IsNotNull(result);
            Assert.AreEqual(RunStatus.Sucessful, result.Status);
            Assert.IsNull(result.Error);
            Assert.AreEqual("test", result.ReturnValue);
        }

        [TestMethod]
        public void ErroredHelper()
        {
            var result = RunResult.Errored(new Exception("test"));
            Assert.IsNotNull(result);
            Assert.AreEqual(RunStatus.Errored, result.Status);
            Assert.IsNull(result.ReturnValue);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual("test", result.Error.Message);
        }

        [TestMethod]
        public void ErroredHelperWithNullParameter()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var result = RunResult.Errored(null);
                Assert.Fail($"Should have thrown a '{nameof(ArgumentNullException)}'.");
            });
        }

        [TestMethod]
        public void DefaultStatus()
        {
            var result = new RunResult();
            Assert.AreEqual(RunStatus.Running, result.Status);
            Assert.IsNull(result.Error);
        }

        [TestMethod]
        public void Done()
        {
            var result = new RunResult();
            Assert.AreEqual(RunStatus.Running, result.Status);
            Assert.IsNull(result.Error);

            result.Done();

            Assert.AreEqual(RunStatus.Sucessful, result.Status);
            Assert.IsNull(result.Error);
        }

        [TestMethod]
        public void DoneWithError()
        {
            var result = new RunResult();
            Assert.AreEqual(RunStatus.Running, result.Status);
            Assert.IsNull(result.Error);

            result.Done(new Exception("test"));

            Assert.AreEqual(RunStatus.Errored, result.Status);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual("test", result.Error.Message);
        }
    }
}