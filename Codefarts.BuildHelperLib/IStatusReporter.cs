// <copyright file="IStatusReporter.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper;

using System;

[Flags]
public enum ReportStatusType
{
    Message = 1,
    Progress = 2,
    Error = 4,
}

public interface IStatusReporter
{
    void Report(string message, ReportStatusType type, string category, float progress);
}