// <copyright file="MetaGeneratorCommand.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using Codefarts.BuildHelper.Exceptions;

namespace Codefarts.BuildHelper
{
    using System;
    using System.IO;

    [NamedParameter("source", typeof(string), true, "The source file assembly to extract the metadata from.")]
    [NamedParameter("destination", typeof(string), true, "The destination file where the metadata will be written to.")]
    [NamedParameter("append", typeof(bool), false, "If true will delete contents from the destination before copying. Default is false.")]
    //  [NamedParameter("relativepaths", typeof(bool), false,
    //                  "Specifies weather condition checks will compare against relative paths or full file paths. Default is false.")]
    public class MetaGeneratorCommand : ICommandPlugin
    {
        private IStatusReporter status;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaGeneratorCommand"/> class.
        /// </summary>
        public MetaGeneratorCommand(IStatusReporter status)
        {
            this.status = status ?? throw new ArgumentNullException(nameof(status));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaGeneratorCommand"/> class.
        /// </summary>
        public MetaGeneratorCommand()
        {
        }

        public string Name => "metagenerator";

        public void Run(RunCommandArgs args)
        {
            //  Debugger.Launch();
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var srcPath = args.GetParameter<string>("source", null);
            var destPath = args.GetParameter<string>("destination", null);

            if (string.IsNullOrWhiteSpace(destPath))
            {
                args.Result = RunResult.Errored(new MissingParameterException("destination"));
                return;
            }

            if (string.IsNullOrWhiteSpace(srcPath))
            {
                args.Result = RunResult.Errored(new MissingParameterException("source"));
                return;
            }

            srcPath = srcPath.ReplaceVariableStrings(args.Variables);
            destPath = destPath.ReplaceVariableStrings(args.Variables);

            // validate src and dest paths
            if (srcPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                args.Result = RunResult.Errored(
                    new MissingParameterException("source", "Contains invalid path characters after replacing variables."));
                return;
            }

            if (destPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                args.Result = RunResult.Errored(
                    new MissingParameterException("destination", "Contains invalid path characters after replacing variables."));
                return;
            }

            var isAppending = args.GetParameter("append", false);

            //   this.status?.Report($"Clearing before copy ({doClear}): {destPath}");
            try
            {
                // generate
            }
            catch (Exception ex)
            {
                args.Result = RunResult.Errored(ex);
                return;
            }

            args.Result = RunResult.Sucessful();
        }
    }
}