// <copyright file="StatusCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;

    [NamedParameter("text", typeof(string), false, "The message to be reported.")]
    [NamedParameter("type", typeof(ReportStatusType), false, "The message type.")]
    [NamedParameter("category", typeof(string), false, "The message category.")]
    [NamedParameter("progress", typeof(float), false, "The progress being reported.")]
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

        public string Name => "status";

        public void Run(RunCommandArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            // get the variable name parameter
            var messageValue = args.GetParameter<string>("message", null);
            var typeValue = args.GetParameter<ReportStatusType>("type", ReportStatusType.Message);
            var categoryValue = args.GetParameter<string>("category", null);
            var progressValue = args.GetParameter<float>("progress", 0f);

            this.status.Report(messageValue, typeValue, categoryValue, progressValue);

            args.Result = RunResult.Sucessful();
        }
    }
}