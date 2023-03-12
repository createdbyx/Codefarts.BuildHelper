// <copyright file="StringBuilderStatusReporter.cs" company="Codefarts">
// Copyright (c) Codefarts
// contact@codefarts.com
// http://www.codefarts.com
// </copyright>

using System.Text;

namespace Codefarts.BuildHelper.StringBuilderReporter;

using Codefarts.BuildHelper;

public class StringBuilderStatusReporter : IStatusReporter
{
    public StringBuilder StringBuilder { get; set; } = new();
        
    public string HeaderPrefix { get; set; }

    public void Report(string message, ReportStatusType type, string category, float progress)
    {
        var categoryText = string.IsNullOrWhiteSpace(category) ? string.Empty : $"(Category: {category}) ";
        var headerPrefix = string.IsNullOrWhiteSpace(this.HeaderPrefix) ? string.Empty : $"{this.HeaderPrefix}=> ";
        var typeText = $"(Type: {type}) ";
        var progressText = type.HasFlag(ReportStatusType.Progress) ? $"(Progress: {progress}) " : string.Empty;
        var infoPrefix = $"{headerPrefix}{categoryText}{typeText}{progressText}";
        infoPrefix += !string.IsNullOrEmpty(infoPrefix) ? "- " : infoPrefix;
        this.StringBuilder?.AppendLine(infoPrefix + $"{message}");
    }
}