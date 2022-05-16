// <copyright file="StatusCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;

    [NamedParameter("text", typeof(string), false, "The message to be reported. Default is null.")]
    [NamedParameter("type", typeof(ReportStatusType), false, "The message type. Default is message.")]
    [NamedParameter("category", typeof(string), false, "The message category. Default is null.")]
    [NamedParameter("progress", typeof(float), false, "The progress being reported. Default is zero.")]
    [NamedParameter(null, null)]
    public class StatusCommand : ICommandPlugin
    {
        private IStatusReporter status;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusCommand"/> class.
        /// </summary>
        public StatusCommand(IStatusReporter status)
        {
            this.status = status ?? throw new ArgumentNullException(nameof(status));
        }

        public StatusCommand()
        {
        }

        public string Name => "status";

        public void Run(RunCommandArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            // get the variable name parameter
            var messageValue = args.GetParameter<string>("text", null);
            var typeValue = args.GetParameter<ReportStatusType>("type", ReportStatusType.Message);
            var categoryValue = args.GetParameter<string>("category", null);
            var progressValue = args.GetParameter<float>("progress", 0f);

            messageValue = messageValue?.ReplaceVariableStrings(args.Variables);
            categoryValue = categoryValue?.ReplaceVariableStrings(args.Variables);

            this.status?.Report(messageValue, typeValue, categoryValue, progressValue);

            args.Result = RunResult.Sucessful();
        }
    }
}