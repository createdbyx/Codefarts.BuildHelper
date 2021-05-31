// <copyright file="RunCommandArgsTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System;
using System.Collections.Generic;
using Codefarts.BuildHelper;

namespace BuildHelperTests
{
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
                var args = new ExecuteCommandArgs(null, null, null, null);
            });
        }

        [TestMethod]
        public void NullCommandArg()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var args = new ExecuteCommandArgs(x => { }, new Dictionary<string, string>(), null, new BuildHelper());
            });
        }

        [TestMethod]
        public void NullBuildHelper()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var args = new ExecuteCommandArgs(x => { }, new Dictionary<string, string>(), new CommandData(), null);
            });
        }

        [TestMethod]
        public void NullOutputArg()
        {
            var variables = new Dictionary<string, string>();
            var data = new CommandData();
            var helper = new BuildHelper();
            var args = new ExecuteCommandArgs(null, variables, data, helper);
            Assert.IsNotNull(args.Output);
            Assert.AreSame(variables, args.Variables);
            Assert.AreSame(data, args.Command);
            Assert.AreSame(helper, args.BuildHelper);
            Assert.IsNotNull(args.Parameters);
        }

        [TestMethod]
        public void ValidArgs()
        {
            Action<string> output = x => { };
            var variables = new Dictionary<string, string>();
            var data = new CommandData();
            var helper = new BuildHelper();
            var args = new ExecuteCommandArgs(output, variables, data, helper);
            Assert.IsNotNull(args.Output);
            Assert.AreNotSame(output, args.Output);
            Assert.AreSame(variables, args.Variables);
            Assert.AreSame(data, args.Command);
            Assert.AreSame(helper, args.BuildHelper);
            Assert.IsNotNull(args.Parameters);
        }
    }
}