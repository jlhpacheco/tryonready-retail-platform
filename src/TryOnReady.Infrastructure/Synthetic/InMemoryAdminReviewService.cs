using TryOnReady.Application.AdminReview;

namespace TryOnReady.Infrastructure.Synthetic;

public sealed class InMemoryAdminReviewService : IAdminReviewService
{
    private const string ReviewId = "64cd4178-f847-43fc-b112-3f303b84ce60";
    private readonly Lock syncRoot = new();
    private AdminReviewItem review = new(
        Guid.Parse(ReviewId),
        Guid.Parse("bac012b4-fc08-4f74-a587-4b42fb791906"),
        "Luna & Thread",
        "Moonlight Blazer",
        "SYN-BLZ-001",
        "Full-body outfit",
        "Pending",
        true,
        "Not submitted to YouCam",
        new DateTimeOffset(2026, 7, 25, 16, 30, 0, TimeSpan.Zero),
        null,
        null);

    public Task<IReadOnlyList<AdminReviewItem>> GetReviewsAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (syncRoot)
        {
            return Task.FromResult<IReadOnlyList<AdminReviewItem>>([review]);
        }
    }

    public Task<AdminReviewItem?> RecordDecisionAsync(
        Guid reviewId,
        string decision,
        string? notes,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (syncRoot)
        {
            if (review.Id != reviewId)
            {
                return Task.FromResult<AdminReviewItem?>(null);
            }

            review = review with
            {
                Status = decision,
                DecisionNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
                DecidedAtUtc = DateTimeOffset.UtcNow,
            };

            return Task.FromResult<AdminReviewItem?>(review);
        }
    }
}
