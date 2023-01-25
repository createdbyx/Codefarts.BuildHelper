using System;

namespace Codefarts.BuildHelper;

public static class IStatusReporterExtensionMethods
{

    public static void ReportError(this IStatusReporter status, string message)
    {
        status = status ?? throw new ArgumentNullException(nameof(status));
        status.Report(message, ReportStatusType.Error | ReportStatusType.Message, null, 0);
    }

    public static void ReportError(this IStatusReporter status, string message, float progress)
    {
        status = status ?? throw new ArgumentNullException(nameof(status));
        status.Report(message, ReportStatusType.Error | ReportStatusType.Progress | ReportStatusType.Message, null, progress);
    }

    public static void ReportError(this IStatusReporter status, string message, string category, float progress)
    {
        status = status ?? throw new ArgumentNullException(nameof(status));
        status.Report(message, ReportStatusType.Error | ReportStatusType.Progress | ReportStatusType.Message, category, progress);
    }

    public static void ReportProgress(this IStatusReporter status, string message, float progress)
    {
        status = status ?? throw new ArgumentNullException(nameof(status));
        status.Report(message, ReportStatusType.Message | ReportStatusType.Progress, null, progress);
    }

    public static void ReportProgress(this IStatusReporter status, string message, string category, float progress)
    {
        status = status ?? throw new ArgumentNullException(nameof(status));
        status.Report(message, ReportStatusType.Message | ReportStatusType.Progress, category, progress);
    }

    public static void ReportProgress(this IStatusReporter status, float progress)
    {
        status = status ?? throw new ArgumentNullException(nameof(status));
        status.Report(null, ReportStatusType.Progress, null, progress);
    }

    public static void Report(this IStatusReporter status, string message)
    {
        status = status ?? throw new ArgumentNullException(nameof(status));
        status.Report(message, ReportStatusType.Message, null, 0);
    }

    public static void Report(this IStatusReporter status, string message, string category)
    {
        status = status ?? throw new ArgumentNullException(nameof(status));
        status.Report(message, ReportStatusType.Message, category, 0);
    }
}