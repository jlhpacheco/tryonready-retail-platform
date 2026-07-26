namespace TryOnReady.Application.AdminReview;

public interface IAdminReviewService
{
    Task<IReadOnlyList<AdminReviewItem>> GetReviewsAsync(
        CancellationToken cancellationToken);

    Task<AdminReviewItem?> RecordDecisionAsync(
        Guid reviewId,
        string decision,
        string? notes,
        CancellationToken cancellationToken);
}
