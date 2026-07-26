namespace TryOnReady.Application.TryOn;

public interface ITryOnService
{
    Task<TryOnJobView> SubmitAsync(
        TryOnJobSubmission submission,
        CancellationToken cancellationToken);

    Task<TryOnJobView?> GetAsync(
        Guid jobId,
        CancellationToken cancellationToken);

    Task<TryOnResultContent?> GetResultAsync(
        Guid jobId,
        CancellationToken cancellationToken);

    Task<TryOnDashboard> GetDashboardAsync(
        CancellationToken cancellationToken);
}

public sealed record TryOnJobSubmission(
    Guid ProductId,
    bool ConsentAccepted,
    string ConsentVersion,
    string FileName,
    string MediaType,
    int PixelWidth,
    int PixelHeight,
    byte[] Content);

public sealed record TryOnJobView(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string Status,
    string Message,
    bool IsDuplicate,
    int DuplicateRequestCount,
    int ApiUnitsReserved,
    int ApiUnitsConsumed,
    string? ResultUrl,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    DateTimeOffset? CompletedAtUtc);

public sealed record TryOnResultContent(
    string FileName,
    string MediaType,
    byte[] Content);

public sealed record TryOnDashboard(
    int TotalJobs,
    int PendingJobs,
    int ProcessingJobs,
    int SucceededJobs,
    int FailedJobs,
    int DuplicateRequestsPrevented,
    int ApiUnitsReserved,
    int ApiUnitsConsumed);

