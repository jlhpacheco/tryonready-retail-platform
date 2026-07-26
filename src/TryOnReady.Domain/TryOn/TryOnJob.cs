namespace TryOnReady.Domain.TryOn;

public sealed record TryOnJob(
    Guid Id,
    Guid ProductId,
    TryOnJobStatus Status,
    DateTimeOffset CreatedAtUtc,
    string? ProviderReference);

public enum TryOnJobStatus
{
    Pending,
    Running,
    Succeeded,
    Failed,
}
