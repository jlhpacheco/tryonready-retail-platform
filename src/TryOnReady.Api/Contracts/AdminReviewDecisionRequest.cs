namespace TryOnReady.Api.Contracts;

public sealed record AdminReviewDecisionRequest(
    string? Decision,
    string? Notes);
