// <copyright file="RunCommandArgs.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper;

using System;

public class RunCommandArgs
{
    public RunCommandArgs(VariablesDictionary variables, CommandData command)
    {
        // we wrap output callback here to ensure any call to it does not throw null reference exceptions
        this.Variables = variables ?? new VariablesDictionary();
        this.Command = command ?? throw new ArgumentNullException(nameof(command));
    }

    public RunCommandArgs(CommandData command)
        : this(null, command)
    {
    }

    public CommandData Command { get; }

    public VariablesDictionary Variables { get; }

    public RunResult Result { get; set; }
}