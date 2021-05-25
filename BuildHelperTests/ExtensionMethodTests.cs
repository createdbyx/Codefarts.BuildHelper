// <copyright file="ExtensionMethodTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace BuildHelperTests
{
    using System.Collections.Generic;
    using System.IO;
    using Codefarts.BuildHelper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [TestCategory(nameof(ExtensionMethods))]
    public class ExtensionMethodTests
    {
        [TestMethod]
        public void Null()
        {
            var vars = new Dictionary<string, string>();
            vars["ProjectDir"] = Path.GetTempPath();
            vars["ConfigurationName"] = "DEBUG";
            vars["OutDir"] = "bin";

            string text = null;
            text = text.ReplaceBuildVariableStrings(vars);

            Assert.IsNull(text);
        }

        [TestMethod]
        public void Empty()
        {
            var vars = new Dictionary<string, string>();
            vars["ProjectDir"] = Path.GetTempPath();
            vars["ConfigurationName"] = "DEBUG";
            vars["OutDir"] = "bin";

            var text = string.Empty;
            text = text.ReplaceBuildVariableStrings(vars);

            Assert.AreEqual(string.Empty, text);
        }

        [TestMethod]
        public void Whitespace()
        {
            var vars = new Dictionary<string, string>();
            vars["ProjectDir"] = Path.GetTempPath();
            vars["ConfigurationName"] = "DEBUG";
            vars["OutDir"] = "bin";

            var text = "   ";
            text = text.ReplaceBuildVariableStrings(vars);

            Assert.AreEqual("   ", text);
        }

        [TestMethod]
        public void VarCustomVar()
        {
            var vars = new Dictionary<string, string>();
            var tempPath = Path.GetTempPath();
            vars["ProjectDir"] = tempPath;
            vars["ConfigurationName"] = "DEBUG";
            vars["OutDir"] = "bin";

            var text = @"$(ProjectDir)DeployPath_$(ConfigurationName)";
            text = text.ReplaceBuildVariableStrings(vars);

            var expected = Path.Combine(tempPath, "DeployPath_DEBUG");
            Assert.AreEqual(expected, text);
        }

        [TestMethod]
        public void KeepUnsetVaribles()
        {
            var vars = new Dictionary<string, string>();
            var tempPath = Path.GetTempPath();
            //vars["ProjectDir"] = tempPath;
            //vars["ConfigurationName"] = "DEBUG";
            //  vars["OutDir"] = "bin";

            var text = "$(ProjectDir)";
            text = text.ReplaceBuildVariableStrings(vars);

            var expected = "$(ProjectDir)";
            Assert.AreEqual(expected, text);
        }
    }
}