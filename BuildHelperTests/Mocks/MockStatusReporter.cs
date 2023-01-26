// <copyright file="MockStatusReporter.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace BuildHelperTests.Mocks;

using Codefarts.BuildHelper;

public class MockStatusReporter : IStatusReporter
{
    public int CallCount { get; set; }

    public void Report(string message, ReportStatusType type, string category, float progress)
    {
        this.Message = message;
        this.Type = type;
        this.Category = category;
        this.Progress = progress;
        this.CallCount++;
    }

    public string Message { get; set; }

    public ReportStatusType Type { get; set; }

    public string Category { get; set; }

    public float Progress { get; set; }
}