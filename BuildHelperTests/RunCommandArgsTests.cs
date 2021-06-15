// <copyright file="RunCommandArgsTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace BuildHelperTests
{
    using System;
    using System.Collections.Generic;
    using Codefarts.BuildHelper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, TestCategory("RunCommandArgs")]
    public class RunCommandArgsTests
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
        public void CtorAllNulls()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var args = new RunCommandArgs(null, null);
            });
        }

        [TestMethod]
        public void NullCommandArg()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var args = new RunCommandArgs(new VariablesDictionary(), null);
            });
        }
  

        [TestMethod]
        public void ValidArgs()
        {
            var variables = new VariablesDictionary();
            var data = new CommandData();
            var args = new RunCommandArgs( variables, data);
            Assert.AreSame(variables, args.Variables);
            Assert.AreSame(data, args.Command);
        }
    }
}