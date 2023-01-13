// <copyright file="RunResult.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper;

using System;

public class RunResult : RunResult<object>
{
    public RunResult()
    {
    }

    public RunResult(Exception error) : base(error)
    {
    }

    public RunResult(object returnValue) : base(returnValue)
    {
    }
}