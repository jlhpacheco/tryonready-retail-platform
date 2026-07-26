namespace TryOnReady.Application.AdminReview;

public sealed record AdminReviewItem(
    Guid Id,
    Guid ProductId,
    string BoutiqueName,
    string ProductName,
    string Sku,
    string Category,
    string Status,
    bool ReadinessPassed,
    string ProviderStatus,
    DateTimeOffset SubmittedAtUtc,
    string? DecisionNotes,
    DateTimeOffset? DecidedAtUtc);
