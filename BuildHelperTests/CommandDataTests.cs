// <copyright file="CommandDataTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace BuildHelperTests
{
    using Codefarts.BuildHelper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [TestCategory(nameof(CommandDataTests))]
    public class CommandDataTests
    {
        [TestMethod]
        public void DefaultValues()
        {
            var data = new CommandData();
            Assert.IsNull(data.Name);
            Assert.IsNull(data.Parent);
            Assert.IsNotNull(data.Children);
            Assert.AreEqual(0, data.Children.Count);
            Assert.IsNotNull(data.Parameters);
            Assert.AreEqual(0, data.Parameters.Count);
        }
    }
}