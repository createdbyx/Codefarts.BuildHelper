// <copyright file="ConsoleStatusReporter.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

namespace Codefarts.BuildHelper.ConsoleReporter;

using System;
using Codefarts.BuildHelper;

public class ConsoleStatusReporter : IStatusReporter
{
    public string HeaderPrefix { get; set; }

    public void Report(string message, ReportStatusType type, string category, float progress)
    {
        var categoryText = string.IsNullOrWhiteSpace(category) ? string.Empty : $"(Category: {category}) ";
        var headerPrefix = string.IsNullOrWhiteSpace(this.HeaderPrefix) ? string.Empty : $"{this.HeaderPrefix}=> ";
        var typeText = $"(Type: {type}) ";
        var progressText = type.HasFlag(ReportStatusType.Progress) ? $"(Progress: {progress}) " : string.Empty;
        var infoPrefix = $"{headerPrefix}{categoryText}{typeText}{progressText}";
        infoPrefix += !string.IsNullOrEmpty(infoPrefix) ? "- " : infoPrefix;
        Console.WriteLine(infoPrefix + $"{message}");
    }
}