// <copyright file="BuildHelperTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace BuildHelperTests
{
    using Codefarts.BuildHelper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [TestCategory(nameof(BuildHelper))]
    public class BuildHelperTests
    {
        [TestMethod]
        public void Instanciate()
        {
            var build = new BuildHelper();
        }
    }
}