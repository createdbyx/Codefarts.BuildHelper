// <copyright file="ExtensionMethods.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public static class ExtensionMethods
    {
        public static T GetValue<T>(this IDictionary<string, object> parameters, string name)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            object value;
            if (!parameters.TryGetValue(name, out value))
            {
                throw new KeyNotFoundException(name);
            }

            if (value is IConvertible)
            {
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }

            return (T)value;
        }

        public static T GetValue<T>(this IDictionary<string, object> parameters, string name, T defaultValue)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            object value;
            if (parameters.TryGetValue(name, out value))
            {
                try
                {
                    if (value is IConvertible)
                    {
                        return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
                    }

                    return (T)value;
                }
                catch
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        public static T GetReturnValue<T>(this RunResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var value = result.ReturnValue;
            if (value is IConvertible)
            {
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }

            return (T)value;
        }

        public static T GetReturnValue<T>(this RunResult result, T defaultValue)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            try
            {
                var value = result.ReturnValue;
                if (value is IConvertible)
                {
                    return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
                }

                return (T)value;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static T GetParameter<T>(this RunCommandArgs args, string name)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            object value;
            if (!args.Command.Parameters.TryGetValue(name, out value))
            {
                throw new MissingParameterException(name);
            }

            if (value is IConvertible)
            {
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }

            return (T)value;
        }

        public static T GetParameter<T>(this RunCommandArgs args, string name, T defaultValue)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            return args.Command.Parameters.GetValue<T>(name, defaultValue);
        }

        public static T GetVariable<T>(this RunCommandArgs args, string name)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            object value;
            if (!args.Variables.TryGetValue(name, out value))
            {
                throw new MissingVariableException(name);
            }

            if (value is IConvertible)
            {
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }

            return (T)value;
        }

        public static T GetVariable<T>(this RunCommandArgs args, string name, T defaultValue)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            object value;
            if (args.Variables.TryGetValue(name, out value))
            {
                try
                {
                    if (value is IConvertible)
                    {
                        return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
                    }

                    return (T)value;
                }
                catch
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        public static T GetParameter<T>(this CommandData commandData, string name, T defaultValue)
        {
            if (commandData == null)
            {
                throw new ArgumentNullException(nameof(commandData));
            }

            return commandData.Parameters.GetValue(name, defaultValue);
        }

        public static T GetParameter<T>(this CommandData commandData, string name)
        {
            return commandData.Parameters.GetValue<T>(name, default);
        }

        public static string ReplaceVariableStrings(this string text, IDictionary<string, object> variables)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            foreach (var item in variables)
            {
                if (item.Value != null)
                {
                    text = text.Replace($"$({item.Key})", item.Value.ToString());
                }
            }

            return text;
        }

        public static bool SatifiesConditions(this CommandData data, IDictionary<string, object> variables)
        {
            return SatifiesConditions(data, variables, true);
        }

        public static bool SatifiesConditions(this CommandData data, IDictionary<string, object> variables, string compareValue)
        {
            return data.SatifiesConditions(variables, true, compareValue);
        }

        public static bool SatifiesConditions(this CommandData data, IDictionary<string, object> variables, bool allConditions)
        {
            if (allConditions)
            {
                return data.Children.Where(condition => condition.Name == "condition")
                           .All(condition => condition.SatifiesCondition(variables));
            }

            return data.Children.Where(condition => condition.Name == "condition")
                       .Any(condition => condition.SatifiesCondition(variables));
        }

        public static bool SatifiesConditions(this CommandData data, IDictionary<string, object> variables, bool allConditions,
                                              string compareValue)
        {
            if (allConditions)
            {
                return data.Children.Count == 0 || data.Children.Where(condition => condition.Name == "condition")
                                                       .All(condition => condition.SatifiesCondition(variables, compareValue));
            }

            return data.Children.Count == 0 || data.Children.Where(condition => condition.Name == "condition")
                                                   .Any(condition => condition.SatifiesCondition(variables, compareValue));
        }

        public static bool SatifiesCondition(this CommandData data, IDictionary<string, object> variables)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var value1 = data.GetParameter<string>("value1");
            return SatifiesCondition(data, variables, value1);
        }

        public static bool SatifiesCondition(this CommandData data, IDictionary<string, object> variables, string compareValue)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Name != "condition")
            {
                throw new ArgumentException("CommandData name is not 'condition'.", nameof(data));
            }

            var value2 = data.GetParameter<string>("value2");
            value2 = value2 ?? data.GetParameter<string>("value");
            var operatorValue = data.GetParameter<string>("operator");
            var ignoreCaseValue = data.GetParameter<string>("ignorecase");

            compareValue = variables != null ? compareValue.ReplaceVariableStrings(variables) : compareValue;
            value2 = variables != null ? value2.ReplaceVariableStrings(variables) : value2;
            operatorValue = variables != null ? operatorValue.ReplaceVariableStrings(variables) : operatorValue;
            ignoreCaseValue = variables != null ? ignoreCaseValue.ReplaceVariableStrings(variables) : ignoreCaseValue;

            var ignoreCase = true;
            if (ignoreCaseValue != null && !bool.TryParse(ignoreCaseValue, out ignoreCase))
            {
                throw new ArgumentOutOfRangeException("'ignorecase' attribute exists but it's value could not be parsed as a bool value.");
            }

            return SatifiesCondition(compareValue, value2, operatorValue, ignoreCase);
        }

        public static bool SatifiesCondition(string value1, string value2, string operatorValue, bool ignoreCase)
        {
            if (value1 == null)
            {
                throw new ArgumentOutOfRangeException("'value1' is missing.", nameof(value1));
            }

            if (value2 == null)
            {
                throw new ArgumentOutOfRangeException("'value2' is missing.", nameof(value2));
            }

            operatorValue = operatorValue == null ? operatorValue : operatorValue.ToLowerInvariant().Trim();
            switch (operatorValue)
            {
                case "=":
                case "equals":
                case "equalto":
                    if (ignoreCase ? string.Equals(value1, value2, StringComparison.OrdinalIgnoreCase) : string.Equals(value1, value2))
                    {
                        return true;
                    }

                    return false;

                case "!=":
                case "notequal":
                case "notequalto":
                    if (ignoreCase ? !string.Equals(value1, value2, StringComparison.OrdinalIgnoreCase) : !string.Equals(value1, value2))
                    {
                        return true;
                    }

                    return false;

                case "startswith":
                case "beginswith":
                    if (ignoreCase ? value1.StartsWith(value2, StringComparison.OrdinalIgnoreCase) : value1.StartsWith(value2))
                    {
                        return true;
                    }

                    return false;

                case "endswith":
                    if (ignoreCase ? value1.EndsWith(value2, StringComparison.OrdinalIgnoreCase) : value1.EndsWith(value2))
                    {
                        return true;
                    }

                    return false;

                case "contains":
                    if (ignoreCase ? value1.IndexOf(value2, StringComparison.OrdinalIgnoreCase) >= 0 : value1.Contains(value2))
                    {
                        return true;
                    }

                    return false;

                case "missing":
                case "ismissing":
                case "notfound":
                case "!contains":
                    if (ignoreCase ? value1.IndexOf(value2, StringComparison.OrdinalIgnoreCase) >=0 : value1.Contains(value2))
                    {
                        return false;
                    }

                    return true;

                case null:
                    throw new ArgumentNullException("Condition is missing 'operator' value.");

                default:
                    throw new ArgumentOutOfRangeException("'operator' attribute exists but it's meaning could not determined.");
            }
        }

        public static void ReportError(this IStatusReporter status, string message)
        {
            status = status ?? throw new ArgumentNullException(nameof(status));
            status.Report(message, ReportStatusType.Error | ReportStatusType.Message, null, 0);
        }

        public static void ReportError(this IStatusReporter status, string message, float progress)
        {
            status = status ?? throw new ArgumentNullException(nameof(status));
            status.Report(message, ReportStatusType.Error | ReportStatusType.Progress | ReportStatusType.Message, null, progress);
        }

        public static void ReportError(this IStatusReporter status, string message, string category, float progress)
        {
            status = status ?? throw new ArgumentNullException(nameof(status));
            status.Report(message, ReportStatusType.Error | ReportStatusType.Progress | ReportStatusType.Message, category, progress);
        }

        public static void ReportProgress(this IStatusReporter status, string message, float progress)
        {
            status = status ?? throw new ArgumentNullException(nameof(status));
            status.Report(message, ReportStatusType.Message | ReportStatusType.Progress, null, progress);
        }

        public static void ReportProgress(this IStatusReporter status, string message, string category, float progress)
        {
            status = status ?? throw new ArgumentNullException(nameof(status));
            status.Report(message, ReportStatusType.Message | ReportStatusType.Progress, category, progress);
        }

        public static void ReportProgress(this IStatusReporter status, float progress)
        {
            status = status ?? throw new ArgumentNullException(nameof(status));
            status.Report(null, ReportStatusType.Progress, null, progress);
        }

        public static void Report(this IStatusReporter status, string message)
        {
            status = status ?? throw new ArgumentNullException(nameof(status));
            status.Report(message, ReportStatusType.Message, null, 0);
        }

        public static void Report(this IStatusReporter status, string message, string category)
        {
            status = status ?? throw new ArgumentNullException(nameof(status));
            status.Report(message, ReportStatusType.Message, category, 0);
        }

        public static void Run(
            this IEnumerable<CommandData> commands,
            VariablesDictionary variables,
            PluginCollection plugins,
            IStatusReporter status)
        {
            Run(commands, variables, plugins, status, null);
        }

        public static void Run(
            this IEnumerable<CommandData> commands,
            VariablesDictionary variables,
            PluginCollection plugins,
            IStatusReporter status,
            Action<RunCommandArgs> resultCallback)
        {
            commands = commands ?? throw new ArgumentNullException(nameof(commands));
            variables = variables ?? throw new ArgumentNullException(nameof(variables));
            plugins = plugins ?? throw new ArgumentNullException(nameof(plugins));

            if (plugins.Count == 0)
            {
                return;
            }

            // process file elements
            foreach (var command in commands)
            {
                // find the first plugin with the matching name
                var plugin = plugins.FirstOrDefault(c => c.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase));
                if (plugin == null)
                {
                    continue;
                }

                var args = command.Run(variables, plugin, status);
                resultCallback?.Invoke(args);
            }
        }

        public static RunCommandArgs Run(this CommandData command, ICommandPlugin plugin, IStatusReporter status)
        {
            return Run(command, new VariablesDictionary(), plugin, status);
        }

        public static RunCommandArgs Run(
            this CommandData command,
            VariablesDictionary variables,
            ICommandPlugin plugin,
            IStatusReporter status)
        {
            command = command ?? throw new ArgumentNullException(nameof(command));
            variables = variables ?? throw new ArgumentNullException(nameof(variables));
            plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));

            // setup executing args
            var executeCommandArgs = new RunCommandArgs(variables, command);
            try
            {
                // // check if the command has an attached message and if so output the message before executing
                // var message = command.GetParameter("message", string.Empty);
                // if (!string.IsNullOrWhiteSpace(message))
                // {
                //     message = message.ReplaceVariableStrings(variables);
                //     status?.Report($"Message: {message}");
                // }

                // execute the command plugin
                plugin.Run(executeCommandArgs);
                var result = executeCommandArgs.Result;

                if (result != null)
                {
                    status?.Report($"'{command.Name}' completed with status '{result.Status}'.");
                    if (result.Status == RunStatus.Errored)
                    {
                        status?.Report(result.Error.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                status?.Report($"Command {plugin.Name} threw an unexpected exception.");
                status?.Report(ex.ToString());
                throw ex;
            }

            return executeCommandArgs;
        }
    }
}