// <copyright file="ExtensionMethodTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace BuildHelperTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Codefarts.BuildHelper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [TestCategory("Extension Methods")]
    public class ExtensionMethodTests
    {
        [TestMethod]
        public void ReplaceVariableStringsNull()
        {
            var vars = new Dictionary<string, object>();
            vars["ProjectDir"] = Path.GetTempPath();
            vars["ConfigurationName"] = "DEBUG";
            vars["OutDir"] = "bin";

            string text = null;
            text = ExtensionMethods.ReplaceVariableStrings(text, vars);

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
            text = ExtensionMethods.ReplaceVariableStrings(text, vars);

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
            text = ExtensionMethods.ReplaceVariableStrings(text, vars);

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
            text = ExtensionMethods.ReplaceVariableStrings(text, vars);

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
            text = ExtensionMethods.ReplaceVariableStrings(text, vars);

            var expected = "$(ProjectDir)";
            Assert.AreEqual(expected, text);
        }

        [TestMethod]
        public void GetValueWithNullParameters()
        {
            Assert.ThrowsException<ArgumentNullException>(() => { ExtensionMethods.GetValue<string>(null, "test", string.Empty); });
        }

        [TestMethod]
        public void RunCommandArgsGetParameterWithNullArgsAndDefaultValue()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                RunCommandArgs args = null;
                ExtensionMethods.GetParameter<string>(args, "test", string.Empty);
            });
        }

        [TestMethod]
        public void RunCommandArgsGetParameterWithMissingParameterAndDefaultValue()
        {
            var data = new CommandData();
            var args = new RunCommandArgs(null, new Dictionary<string, object>(), data, new BuildHelper());
            var value = ExtensionMethods.GetParameter<string>(args, "test", "default");
            Assert.AreEqual("default", value);
        }

        [TestMethod]
        public void RunCommandArgsGetParameterWithValidParameterAndNoDefaultValue()
        {
            var data = new CommandData();
            data.Parameters["test"] = "value";
            var args = new RunCommandArgs(null, new Dictionary<string, object>(), data, new BuildHelper());
            var value = ExtensionMethods.GetParameter<string>(args, "test");
            Assert.AreEqual("value", value);
        }

        [TestMethod]
        public void RunCommandArgsGetParameterWithValidParameterAndNoDefaultValueButBadReturnType()
        {
            var data = new CommandData();
            data.Parameters["test"] = "value";
            var args = new RunCommandArgs(null, new Dictionary<string, object>(), data, new BuildHelper());
            Assert.ThrowsException<FormatException>(() => { ExtensionMethods.GetParameter<bool>(args, "test"); });
        }

        [TestMethod]
        public void RunCommandArgsGetParameterWithMissingParameterAndNoDefaultValue()
        {
            var data = new CommandData();
            var args = new RunCommandArgs(null, new Dictionary<string, object>(), data, new BuildHelper());
            Assert.ThrowsException<MissingParameterException>(() => { ExtensionMethods.GetParameter<string>(args, "test"); });
        }

        [TestMethod]
        public void RunCommandArgsGetParameterWithMissingArgsAndNoDefaultValue()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                RunCommandArgs args = null;
                ExtensionMethods.GetParameter<string>(args, "test");
            });
        }

        [TestMethod]
        public void CommandDataGetParameterWithNullArgsAndDefaultValue()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                CommandData args = null;
                ExtensionMethods.GetParameter<string>(args, "test", string.Empty);
            });
        }

        [TestMethod]
        public void CommandDataGetParameterWithValidArgsAndNoDefaultValue()
        {
            var args = new CommandData();
            args.Parameters["test"] = "value";
            var value = ExtensionMethods.GetParameter<string>(args, "test");
            Assert.AreEqual("value", value);
        }

        [TestMethod]
        public void CommandDataGetParameterWithValidArgsAndDefaultValue()
        {
            var args = new CommandData();
            var value = ExtensionMethods.GetParameter<string>(args, "test", "default");
            Assert.AreEqual("default", value);
        }

        [TestMethod]
        public void GetValueWithNullArgsAndDefaultValue()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                IDictionary<string, object> dic = null;
                ExtensionMethods.GetValue<string>(dic, "test", string.Empty);
            });
        }

        [TestMethod]
        public void GetValueWithNullArgsAndNoDefaultValue()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                IDictionary<string, object> dic = null;
                ExtensionMethods.GetValue<string>(dic, "test");
            });
        }

        [TestMethod]
        public void GetValueWithValidArgsAndNoDefaultValue()
        {
            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                var dic = new Dictionary<string, object>();
                ExtensionMethods.GetValue<string>(dic, "test");
            });
        }

        [TestMethod]
        public void GetValueWithValidArgsAndDefaultValue()
        {
            var dic = new Dictionary<string, object>();
            var value = ExtensionMethods.GetValue<string>(dic, "test", "default");
            Assert.AreEqual("default", value);
        }

        [TestMethod]
        public void GetValueWithValidArgsExistingKeyAndNoDefaultValue()
        {
            var dic = new Dictionary<string, object>();
            dic["test"] = "value";
            var value = ExtensionMethods.GetValue<string>(dic, "test");
            Assert.AreEqual("value", value);
        }

        [TestMethod]
        public void GetValueWithValidArgsExistingKeyAndNoDefaultValueBadCasting()
        {
            var dic = new Dictionary<string, object>();
            dic["test"] = "value";
            Assert.ThrowsException<FormatException>(() => { ExtensionMethods.GetValue<bool>(dic, "test"); });
        }

        [TestMethod]
        public void GetReturnValueWithNullArgsNoDefaultValue()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                RunResult result = null;
                ExtensionMethods.GetReturnValue<bool>(result);
            });
        }

        [TestMethod]
        public void GetReturnValueWithNullArgsWithDefaultValue()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                RunResult result = null;
                ExtensionMethods.GetReturnValue<string>(result, "default");
            });
        }

        [TestMethod]
        public void GetReturnValueWithValidArgsBadCastingNoDefaultValue()
        {
            var result = RunResult.Sucessful("test");
            Assert.ThrowsException<InvalidCastException>(() => { ExtensionMethods.GetReturnValue<bool>(result); });
        }

        [TestMethod]
        public void GetReturnValueWithValidArgsBadCastingWithDefaultValue()
        {
            var result = RunResult.Sucessful(true);
            var value = ExtensionMethods.GetReturnValue<string>(result, "default");
            Assert.AreEqual("default", value);
        }

        [TestMethod]
        public void GetVariableWithNullArgsAndDefaultValue()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                RunCommandArgs args = null;
                ExtensionMethods.GetVariable<string>(args, "test", string.Empty);
            });
        }

        [TestMethod]
        public void GetVariableWithNullArgsAndNoDefaultValue()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                RunCommandArgs args = null;
                ExtensionMethods.GetVariable<string>(args, "test");
            });
        }

        [TestMethod]
        public void GetVariableThatReturnsDefaultValue()
        {
            var args = new RunCommandArgs(null, new Dictionary<string, object>(), new CommandData(), new BuildHelper());
            var value = ExtensionMethods.GetVariable<string>(args, "test", "defaultValue");
            Assert.AreEqual("defaultValue", value);
        }

        [TestMethod]
        public void GetMissingVariableWithoutDefaultValue()
        {
            var args = new RunCommandArgs(null, new Dictionary<string, object>(), new CommandData(), new BuildHelper());
            Assert.ThrowsException<MissingVariableException>(() =>
            {
                var value = ExtensionMethods.GetVariable<string>(args, "test");
            });
        }

        [TestMethod]
        public void GetValidVariableWithoutDefaultValue()
        {
            var variables = new Dictionary<string, object>();
            variables["test"] = "someValue";
            var args = new RunCommandArgs(null, variables, new CommandData(), new BuildHelper());

            var value = ExtensionMethods.GetVariable<string>(args, "test");
            Assert.AreEqual("someValue", value);
        }

        [TestMethod]
        public void GetValidVariableWithBadCastingType()
        {
            var variables = new Dictionary<string, object>();
            variables["test"] = "value";
            var args = new RunCommandArgs(null, variables, new CommandData(), new BuildHelper());
            Assert.ThrowsException<FormatException>(() =>
            {
                var value = ExtensionMethods.GetVariable<int>(args, "test");
            });
        }
    }
}