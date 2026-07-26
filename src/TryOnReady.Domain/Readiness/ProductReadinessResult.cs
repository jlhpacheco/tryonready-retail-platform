namespace TryOnReady.Domain.Readiness;

public sealed record ProductReadinessResult(
    bool IsReady,
    IReadOnlyList<ReadinessIssue> Issues)
{
    public static ProductReadinessResult Ready { get; } =
        new(true, Array.Empty<ReadinessIssue>());
}

public sealed record ReadinessIssue(
    string Code,
    string Message,
    ReadinessSeverity Severity);

public enum ReadinessSeverity
{
    Information,
    Warning,
    Blocking,
}
