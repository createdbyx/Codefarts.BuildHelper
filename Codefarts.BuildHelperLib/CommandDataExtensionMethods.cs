using System;
using System.Collections.Generic;
using System.Linq;

namespace Codefarts.BuildHelper;

public static class CommandDataExtensionMethods
{
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

        return ExtensionMethods.SatifiesCondition(compareValue, value2, operatorValue, ignoreCase);
    }

    public static void Run(
        this IEnumerable<CommandData> commands,
        VariablesDictionary variables,
        IEnumerable<ICommandPlugin> plugins,
        IStatusReporter status)
    {
        Run(commands, variables, plugins, status, null);
    }

    public static void Run(
        this IEnumerable<CommandData> commands,
        VariablesDictionary variables,
        IEnumerable<ICommandPlugin> plugins,
        IStatusReporter status,
        Action<RunCommandArgs> resultCallback)
    {
        commands = commands ?? throw new ArgumentNullException(nameof(commands));
        variables = variables ?? throw new ArgumentNullException(nameof(variables));
        plugins = plugins ?? throw new ArgumentNullException(nameof(plugins));

        // process file elements
        foreach (var command in commands)
        {
            var args = command.Run(variables, plugins, status);
            resultCallback?.Invoke(args);
        }
    }

    public static RunCommandArgs Run(this CommandData command, VariablesDictionary variables, IEnumerable<ICommandPlugin> plugins,
                                     IStatusReporter status)
    {
        // find the first plugin with the matching name
        var plugin = plugins?.FirstOrDefault(x => x.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase));
        if (plugin != null)
        {
            return command.Run(variables, plugin, status);
        }

        return new RunCommandArgs(command)
        {
            Result = RunResult.Sucessful()
        };
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
      //  variables = variables ?? throw new ArgumentNullException(nameof(variables));
        plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));

        // setup executing args
        var executeCommandArgs = new RunCommandArgs(variables, command);
        try
        {
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