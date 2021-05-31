// <copyright file="RunCommandArgs.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;
    using System.Collections.Generic;

    public class RunCommandArgs
    {
        public RunCommandArgs(Action<string> output, IDictionary<string, object> variables, CommandData command, BuildHelper buildHelper)
        {
            // we wrap output callback here to ensure any call to it does not throw null reference exceptions
            this.Output = msg =>
            {
                if (output != null)
                {
                    output(msg);
                }
            };

            this.Variables = variables ?? new Dictionary<string, object>();
            this.Command = command ?? throw new ArgumentNullException(nameof(command));
            this.BuildHelper = buildHelper ?? throw new ArgumentNullException(nameof(buildHelper));
        }

        public BuildHelper BuildHelper { get; }

        public CommandData Command { get; }

        public Action<string> Output { get; }

        public IDictionary<string, object> Variables { get; }
    }
}