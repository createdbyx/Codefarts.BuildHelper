// <copyright file="RunResult.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper
{
    using System;

    public class RunResult
    {
        public RunResult()
        {
            this.Status = RunStatus.Running;
        }

        public RunResult(Exception error)
        {
            this.Error = error ?? throw new ArgumentNullException(nameof(error));
            this.Status = RunStatus.Errored;
        }

        public RunResult(object returnValue)
        {
            this.ReturnValue = returnValue;
            this.Status = RunStatus.Sucessful;
        }

        public Exception Error { get; private set; }

        public RunStatus Status { get; private set; }

        public object ReturnValue { get; }

        public static RunResult Sucessful()
        {
            var value = new RunResult();
            value.Status = RunStatus.Sucessful;
            return value;
        }

        public static RunResult Sucessful(object returnValue)
        {
            return new RunResult(returnValue);
        }

        public static RunResult Errored(Exception error)
        {
            return new RunResult(error);
        }

        public void Done()
        {
            this.Status = RunStatus.Sucessful;
        }

        public void Done(Exception error)
        {
            this.Error = error ?? throw new ArgumentNullException(nameof(error));
            this.Status = RunStatus.Errored;
        }
    }
}