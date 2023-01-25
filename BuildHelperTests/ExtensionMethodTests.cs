// <copyright file="ExtensionMethodTests.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using Codefarts.BuildHelper.Exceptions;

namespace BuildHelperTests
{
    using System;
    using BuildHelperTests.Mocks;
    using Codefarts.BuildHelper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [TestCategory("Extension Methods")]
    public class ExtensionMethodTests
    {
        [TestMethod]
        public void RunCommandArgsGetParameterWithNullArgsAndDefaultValue()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                RunCommandArgs args = null;
                RunCommandArgsExtensionMethods.GetParameter(args, "test", string.Empty);
            });
        }

        [TestMethod]
        public void RunCommandArgsGetParameterWithMissingParameterAndDefaultValue()
        {
            var data = new CommandData();
            var args = new RunCommandArgs(new VariablesDictionary(), data);
            var value = RunCommandArgsExtensionMethods.GetParameter(args, "test", "default");
            Assert.AreEqual("default", value);
        }

        [TestMethod]
        public void RunCommandArgsGetParameterWithValidParameterAndNoDefaultValue()
        {
            var data = new CommandData();
            data.Parameters["test"] = "value";
            var args = new RunCommandArgs(new VariablesDictionary(), data);
            var value = RunCommandArgsExtensionMethods.GetParameter<string>(args, "test");
            Assert.AreEqual("value", value);
        }

        [TestMethod]
        public void RunCommandArgsGetParameterWithValidParameterAndNoDefaultValueButBadReturnType()
        {
            var data = new CommandData();
            data.Parameters["test"] = "value";
            var args = new RunCommandArgs(new VariablesDictionary(), data);
            Assert.ThrowsException<FormatException>(() => { RunCommandArgsExtensionMethods.GetParameter<bool>(args, "test"); });
        }

        [TestMethod]
        public void RunCommandArgsGetParameterWithMissingParameterAndNoDefaultValue()
        {
            var data = new CommandData();
            var args = new RunCommandArgs(new VariablesDictionary(), data);
            Assert.ThrowsException<MissingParameterException>(() => { RunCommandArgsExtensionMethods.GetParameter<string>(args, "test"); });
        }

        [TestMethod]
        public void RunCommandArgsGetParameterWithMissingArgsAndNoDefaultValue()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                RunCommandArgs args = null;
                RunCommandArgsExtensionMethods.GetParameter<string>(args, "test");
            });
        }

        [TestMethod]
        public void CommandDataGetParameterWithNullArgsAndDefaultValue()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                CommandData args = null;
                CommandDataExtensionMethods.GetParameter(args, "test", string.Empty);
            });
        }

        [TestMethod]
        public void CommandDataGetParameterWithValidArgsAndNoDefaultValue()
        {
            var args = new CommandData();
            args.Parameters["test"] = "value";
            var value = CommandDataExtensionMethods.GetParameter<string>(args, "test");
            Assert.AreEqual("value", value);
        }

        [TestMethod]
        public void CommandDataGetParameterWithValidArgsAndDefaultValue()
        {
            var args = new CommandData();
            var value = CommandDataExtensionMethods.GetParameter(args, "test", "default");
            Assert.AreEqual("default", value);
        }

        [TestMethod]
        public void GetParameterThatDoesNotImplementIConvertableWithoutDefaultValue()
        {
            Assert.ThrowsException<InvalidCastException>(() =>
            {
                var dic = new VariablesDictionary();
                var com = new CommandData("Command");
                com.Parameters["test"] = new Random();
                var args = new RunCommandArgs(dic, com);
                var value = RunCommandArgsExtensionMethods.GetParameter<bool>(args, "test");
                Assert.Fail($"Should have thrown {nameof(InvalidCastException)}.");
            });
        }

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
        public void GetVariableThatDoesNotImplementIConvertableWithoutDefaultValue()
        {
            Assert.ThrowsException<InvalidCastException>(() =>
            {
                var dic = new VariablesDictionary();
                var com = new CommandData("Command");
                dic["test"] = new Random();
                var args = new RunCommandArgs(dic, com);
                var value = RunCommandArgsExtensionMethods.GetVariable<bool>(args, "test");
                Assert.Fail($"Should have thrown {nameof(InvalidCastException)}.");
            });
        }

        [TestMethod]
        public void GetVariableThatDoesNotImplementIConvertableWithADefaultValue()
        {
            var dic = new VariablesDictionary();
            var com = new CommandData("Command");
            dic["test"] = new Random();
            var args = new RunCommandArgs(dic, com);
            var value = RunCommandArgsExtensionMethods.GetVariable<bool>(args, "test", true);
            Assert.IsTrue(value);
        }

        [TestMethod]
        public void GetVariableThatIsMissingWithADefaultValue()
        {
            var dic = new VariablesDictionary();
            var com = new CommandData("Command");
            dic["test"] = new Random();
            var args = new RunCommandArgs(dic, com);
            var value = RunCommandArgsExtensionMethods.GetVariable<bool>(args, "missing", true);
            Assert.IsTrue(value);
        }

        [TestMethod]
        public void GetVariableThatIsIConvertableWithADefaultValue()
        {
            var dic = new VariablesDictionary();
            var com = new CommandData("Command");
            dic["test"] = 82;
            var args = new RunCommandArgs(dic, com);
            var value = RunCommandArgsExtensionMethods.GetVariable<string>(args, "test", "24");
            Assert.AreEqual("82", value);
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

        [TestMethod]
        public void GetVariableWithNullArgsAndDefaultValue()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                RunCommandArgs args = null;
                RunCommandArgsExtensionMethods.GetVariable(args, "test", string.Empty);
            });
        }

        [TestMethod]
        public void GetVariableWithNullArgsAndNoDefaultValue()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                RunCommandArgs args = null;
                RunCommandArgsExtensionMethods.GetVariable<string>(args, "test");
            });
        }

        [TestMethod]
        public void GetVariableThatReturnsDefaultValue()
        {
            var args = new RunCommandArgs(new VariablesDictionary(), new CommandData());
            var value = RunCommandArgsExtensionMethods.GetVariable(args, "test", "defaultValue");
            Assert.AreEqual("defaultValue", value);
        }

        [TestMethod]
        public void GetMissingVariableWithoutDefaultValue()
        {
            var args = new RunCommandArgs(new VariablesDictionary(), new CommandData());
            Assert.ThrowsException<MissingVariableException>(() =>
            {
                var value = RunCommandArgsExtensionMethods.GetVariable<string>(args, "test");
            });
        }

        [TestMethod]
        public void GetValidVariableWithoutDefaultValue()
        {
            var variables = new VariablesDictionary();
            variables["test"] = "someValue";
            var args = new RunCommandArgs(variables, new CommandData());

            var value = RunCommandArgsExtensionMethods.GetVariable<string>(args, "test");
            Assert.AreEqual("someValue", value);
        }

        [TestMethod]
        public void GetValidVariableWithBadCastingType()
        {
            var variables = new VariablesDictionary();
            variables["test"] = "value";
            var args = new RunCommandArgs(variables, new CommandData());
            Assert.ThrowsException<FormatException>(() =>
            {
                var value = RunCommandArgsExtensionMethods.GetVariable<int>(args, "test");
            });
        }

        [TestMethod]
        public void SatisfiesCondition_WithNullCommandData()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var value = CommandDataExtensionMethods.SatifiesCondition(null, new VariablesDictionary(), "test");
            });
        }

        [TestMethod]
        public void SatisfiesCondition_InvalidCommandDataName()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var com = new CommandData("test");
                var value = CommandDataExtensionMethods.SatifiesCondition(com, new VariablesDictionary(), "test");
            });
        }

        [TestMethod]
        public void SatisfiesCondition_NullCommandData()
        {
            Assert.ThrowsException<ArgumentNullException>(() => CommandDataExtensionMethods.SatifiesCondition(null, new VariablesDictionary()));
        }

        [TestMethod]
        public void SatisfiesCondition_ValidEqualityComparison_IgnoreCase()
        {
            var com = new CommandData("condition");
            com.Parameters["value1"] = "one";
            com.Parameters["operator"] = "=";
            com.Parameters["value2"] = "one";
            com.Parameters["ignorecase"] = "true";
            var value = CommandDataExtensionMethods.SatifiesCondition(com, new VariablesDictionary());
            Assert.IsTrue(value);
        }

        [TestMethod]
        public void ReportErrorResultJustMessageWithNullStatus()
        {
            Assert.ThrowsException<ArgumentNullException>(() => IStatusReporterExtensionMethods.ReportError(null, "test"));
        }

        [TestMethod]
        public void ReportErrorResultJustMessage()
        {
            var report = new MockStatusReporter();
            report.ReportError("test");
            Assert.AreEqual("test", report.Message);
            Assert.IsNull(report.Category);
            Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Message));
            Assert.IsFalse(report.Type.HasFlag(ReportStatusType.Progress));
            Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Error));
        }

        [TestMethod]
        public void ReportErrorResultWithMessageAndProgress()
        {
            var report = new MockStatusReporter();
            report.ReportError("test", 25);
            Assert.AreEqual("test", report.Message);
            Assert.IsNull(report.Category);
            Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Message));
            Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Progress));
            Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Error));
        }

        [TestMethod]
        public void ReportErrorResultWithMessageCategoryProgress()
        {
            var report = new MockStatusReporter();
            report.ReportError("test", "cat", 25);
            Assert.AreEqual("test", report.Message);
            Assert.AreEqual("cat", report.Category);
            Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Message));
            Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Progress));
            Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Error));
        }

        [TestMethod]
        public void ReportProgressResultWithMessageProgress()
        {
            var report = new MockStatusReporter();
            report.ReportProgress("test", 25);
            Assert.AreEqual("test", report.Message);
            Assert.IsNull(report.Category);
            Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Message));
            Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Progress));
            Assert.IsFalse(report.Type.HasFlag(ReportStatusType.Error));
        }

        [TestMethod]
        public void ReportProgressResultWithMessageCategoryProgress()
        {
            var report = new MockStatusReporter();
            report.ReportProgress("test", "cat", 25);
            Assert.AreEqual("test", report.Message);
            Assert.AreEqual("cat", report.Category);
            Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Message));
            Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Progress));
            Assert.IsFalse(report.Type.HasFlag(ReportStatusType.Error));
        }

        [TestMethod]
        public void ReportProgressResultWithProgress()
        {
            var report = new MockStatusReporter();
            report.ReportProgress(25);
            Assert.IsNull(report.Message);
            Assert.IsNull(report.Category);
            Assert.IsFalse(report.Type.HasFlag(ReportStatusType.Message));
            Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Progress));
            Assert.IsFalse(report.Type.HasFlag(ReportStatusType.Error));
        }

        [TestMethod]
        public void ReportMessage()
        {
            var report = new MockStatusReporter();
            report.Report("test");
            Assert.AreEqual("test", report.Message);
            Assert.IsNull(report.Category);
            Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Message));
            Assert.IsFalse(report.Type.HasFlag(ReportStatusType.Progress));
            Assert.IsFalse(report.Type.HasFlag(ReportStatusType.Error));
        }

        [TestMethod]
        public void ReportMessageCategory()
        {
            var report = new MockStatusReporter();
            report.Report("test", "cat");
            Assert.AreEqual("test", report.Message);
            Assert.AreEqual("cat", report.Category);
            Assert.IsTrue(report.Type.HasFlag(ReportStatusType.Message));
            Assert.IsFalse(report.Type.HasFlag(ReportStatusType.Progress));
            Assert.IsFalse(report.Type.HasFlag(ReportStatusType.Error));
        }

        [TestMethod]
        public void RunCommandArrayWithNullArgs()
        {
            CommandData[] commands = null;
            var ex = Assert.ThrowsException<ArgumentNullException>(() => CommandDataExtensionMethods.Run(commands, null, null, null));
            Assert.AreEqual("commands", ex.ParamName);
        }

        [TestMethod]
        public void RunCommandArrayWithNullArgsButValidCommands()
        {
            var commands = new[]
            {
                new CommandData("One"),
                new CommandData("Two"),
                new CommandData("Three"),
            };

            var ex = Assert.ThrowsException<ArgumentNullException>(() => CommandDataExtensionMethods.Run(commands, null, null, null));
            Assert.AreEqual("variables", ex.ParamName);
        }

        [TestMethod]
        public void RunCommandArrayWithNullArgsButValidCommandsAndVariables()
        {
            var commands = new[]
            {
                new CommandData("One"),
                new CommandData("Two"),
                new CommandData("Three"),
            };

            var ex = Assert.ThrowsException<ArgumentNullException>(
                () => CommandDataExtensionMethods.Run(commands, new VariablesDictionary(), null, null));
            Assert.AreEqual("plugins", ex.ParamName);
        }

        [TestMethod]
        public void RunCommandArrayWithValidArgumentsNoStatus()
        {
            var commands = new[]
            {
                new CommandData("One"),
                new CommandData("Two"),
                new CommandData("Three"),
            };

            string returnValue = null;
            var variables = new VariablesDictionary();
            var plugins = new PluginCollection();
            plugins.Add(new MockCommandPlugin("One", x => returnValue = x.Command.Name));

            CommandDataExtensionMethods.Run(commands, variables, plugins, null);
            Assert.IsNotNull(returnValue);
            Assert.AreEqual("One", returnValue);
        }

        [TestMethod]
        public void RunCommandArrayWithValidArgumentsNoStatusNoPlugins()
        {
            var commands = new[]
            {
                new CommandData("One"),
                new CommandData("Two"),
                new CommandData("Three"),
            };

            string returnValue = null;
            var variables = new VariablesDictionary();
            var plugins = new PluginCollection();

            CommandDataExtensionMethods.Run(commands, variables, plugins, null);
        }

        // [TestMethod]
        // public void RunCommandWithNullArgs()
        // {
        //     CommandData command = null;
        //     var ex = Assert.ThrowsException<ArgumentNullException>(() => ExtensionMethods.Run(command, null, null, null));
        //     Assert.AreEqual("command", ex.ParamName);
        // }
        //
        // [TestMethod]
        // public void RunCommandWithNullArgsButValidCommand()
        // {
        //     var command = new CommandData("Test");
        //     var ex = Assert.ThrowsException<ArgumentNullException>(() => ExtensionMethods.Run(command, null, null, null));
        //     Assert.AreEqual("variables", ex.ParamName);
        // }
        //
        // [TestMethod]
        // public void RunCommandWithNullArgsButValidCommandAndVariables()
        // {
        //     var command = new CommandData("Test");
        //     var ex = Assert.ThrowsException<ArgumentNullException>(() => ExtensionMethods.Run(command, new VariablesDictionary(), null, null));
        //     Assert.AreEqual("plugin", ex.ParamName);
        // }

        [TestMethod]
        public void RunCommandWithValidArgsButNoStatusReporter()
        {
            var called = false;
            var command = new CommandData("Test");
            var plugin = new MockCommandPlugin("Test", x => called = true);
            CommandDataExtensionMethods.Run(command, new VariablesDictionary(), plugin, null);
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void RunCommandWithValidArgsButNoStatusReporterAndThrowsException()
        {
            var called = false;
            var command = new CommandData("Test");
            var plugin = new MockCommandPlugin("Test", x =>
            {
                throw new NotImplementedException();
#pragma warning disable 162
                called = true;
#pragma warning restore 162
            });

            Assert.ThrowsException<NotImplementedException>(() => CommandDataExtensionMethods.Run(command, new VariablesDictionary(), plugin, null));
            Assert.IsFalse(called);
        }

        [TestMethod]
        public void RunCommandWithValidArgsAndStatusReporterAndThrowsException()
        {
            var called = false;
            var command = new CommandData("Test");
            var reporter = new MockStatusReporter();
            var plugin = new MockCommandPlugin("Test", x =>
            {
                throw new NotImplementedException();
#pragma warning disable 162
                called = true;
#pragma warning restore 162
            });

            Assert.ThrowsException<NotImplementedException>(
                () => CommandDataExtensionMethods.Run(command, new VariablesDictionary(), plugin, reporter));
            Assert.IsFalse(called);
            Assert.AreEqual(2, reporter.CallCount);
        }

        [TestMethod]
        public void RunCommandWithValidArgsAndStatusReporterSucsesful()
        {
            var called = false;
            var command = new CommandData("Test");
            var reporter = new MockStatusReporter();
            var plugin = new MockCommandPlugin("Test", x =>
            {
                x.Result = RunResult.Sucessful();
                called = true;
            });

            var args = CommandDataExtensionMethods.Run(command, new VariablesDictionary(), plugin, reporter);
            Assert.IsTrue(called);
            Assert.AreEqual(1, reporter.CallCount);
            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Sucessful, args.Result.Status);
        }

        [TestMethod]
        public void RunCommandWithValidArgsAndStatusReporterErrored()
        {
            var called = false;
            var command = new CommandData("Test");
            var reporter = new MockStatusReporter();
            var plugin = new MockCommandPlugin("Test", x =>
            {
                x.Result = RunResult.Errored(new NotImplementedException());
                called = true;
            });

            var args = CommandDataExtensionMethods.Run(command, new VariablesDictionary(), plugin, reporter);
            Assert.IsTrue(called);
            Assert.AreEqual(2, reporter.CallCount);
            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Errored, args.Result.Status);
        }


        [TestMethod]
        public void RunCommandWithValidArgsAndNoVariables()
        {
            var called = false;
            var command = new CommandData("Test");
            var reporter = new MockStatusReporter();
            var plugin = new MockCommandPlugin("Test", x =>
            {
                x.Result = RunResult.Sucessful();
                called = true;
            });

            var args = CommandDataExtensionMethods.Run(command, plugin, reporter);
            Assert.IsTrue(called);
            Assert.AreEqual(1, reporter.CallCount);
            Assert.IsNotNull(args.Result);
            Assert.AreEqual(RunStatus.Sucessful, args.Result.Status);
        }
    }
}