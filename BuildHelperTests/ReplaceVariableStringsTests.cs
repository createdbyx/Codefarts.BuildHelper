using System.Collections.Generic;
using System.IO;
using Codefarts.BuildHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildHelperTests;

[TestClass]
[TestCategory("Extension Methods - ReplaceVariableStrings")]
public class ReplaceVariableStringsTests
{
    [TestMethod]
    public void ReplaceVariableStringsNull()
    {
        var vars = new Dictionary<string, object>();
        vars["ProjectDir"] = Path.GetTempPath();
        vars["ConfigurationName"] = "DEBUG";
        vars["OutDir"] = "bin";

        string text = null;
        text = StringExtensionMethods.ReplaceVariableStrings(text, vars);

        Assert.IsNull(text);
    }

    [TestMethod]
    public void ReplaceVariableStringsEmpty()
    {
        var vars = new Dictionary<string, object>();
        vars["ProjectDir"] = Path.GetTempPath();
        vars["ConfigurationName"] = "DEBUG";
        vars["OutDir"] = "bin";

        var text = string.Empty;
        text = StringExtensionMethods.ReplaceVariableStrings(text, vars);

        Assert.AreEqual(string.Empty, text);
    }

    [TestMethod]
    public void ReplaceVariableStringsWhitespace()
    {
        var vars = new Dictionary<string, object>();
        vars["ProjectDir"] = Path.GetTempPath();
        vars["ConfigurationName"] = "DEBUG";
        vars["OutDir"] = "bin";

        var text = "   ";
        text = StringExtensionMethods.ReplaceVariableStrings(text, vars);

        Assert.AreEqual("   ", text);
    }

    [TestMethod]
    public void ReplaceVariableStringsVarCustomVar()
    {
        var vars = new Dictionary<string, object>();
        var tempPath = Path.GetTempPath();
        vars["ProjectDir"] = tempPath;
        vars["ConfigurationName"] = "DEBUG";
        vars["OutDir"] = "bin";

        var text = @"$(ProjectDir)DeployPath_$(ConfigurationName)";
        text = StringExtensionMethods.ReplaceVariableStrings(text, vars);

        var expected = Path.Combine(tempPath, "DeployPath_DEBUG");
        Assert.AreEqual(expected, text);
    }

    [TestMethod]
    public void ReplaceVariableStringsKeepUnsetVaribles()
    {
        var vars = new Dictionary<string, object>();
        var tempPath = Path.GetTempPath();
        //vars["ProjectDir"] = tempPath;
        //vars["ConfigurationName"] = "DEBUG";
        //  vars["OutDir"] = "bin";

        var text = "$(ProjectDir)";
        text = StringExtensionMethods.ReplaceVariableStrings(text, vars);

        var expected = "$(ProjectDir)";
        Assert.AreEqual(expected, text);
    }
}