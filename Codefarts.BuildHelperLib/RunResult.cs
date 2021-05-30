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
        public Exception Error { get; }

        public RunStatus Status { get; }

        public object ReturnValue { get; }

        public RunResult(Exception error)
        {
            this.Error = error;
            this.Status = RunStatus.Errored;
        }

        public RunResult(object returnValue)
        {
            this.ReturnValue = returnValue;
            this.Status = RunStatus.Sucessful;
        }

        public static RunResult Sucessful(object returnValue)
        {
            return new RunResult(returnValue);
        }

        public static RunResult Errored(Exception error)
        {
            return new RunResult(error);
        }
    }
}