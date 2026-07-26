using Microsoft.EntityFrameworkCore;
using TryOnReady.Application.AdminReview;

namespace TryOnReady.Infrastructure.Persistence;

internal sealed class PersistentAdminReviewService(
    IDbContextFactory<TryOnReadyDbContext> dbContextFactory)
    : IAdminReviewService
{
    public async Task<IReadOnlyList<AdminReviewItem>> GetReviewsAsync(
        CancellationToken cancellationToken)
    {
        await using var dbContext =
            await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var products = await dbContext.Products
            .AsNoTracking()
            .Include(product => product.BoutiqueApplication)
            .Include(product => product.TryOnJobs)
            .OrderByDescending(product => product.SubmittedAtUtc)
            .ToArrayAsync(cancellationToken);
        return products.Select(Map).ToArray();
    }

    public async Task<AdminReviewItem?> RecordDecisionAsync(
        Guid reviewId,
        string decision,
        string? notes,
        CancellationToken cancellationToken)
    {
        await using var dbContext =
            await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var product = await dbContext.Products
            .Include(item => item.BoutiqueApplication)
            .Include(item => item.TryOnJobs)
            .SingleOrDefaultAsync(item => item.Id == reviewId, cancellationToken);

        if (product is null)
        {
            return null;
        }

        if (decision == "Approved"
            && product.BoutiqueApplication.Status != "Approved")
        {
            throw new InvalidOperationException(
                "Approve the boutique application before approving its garment.");
        }

        product.Status = decision;
        product.DecisionNotes = string.IsNullOrWhiteSpace(notes)
            ? null
            : notes.Trim();
        product.DecidedAtUtc = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(product);
    }

    private static AdminReviewItem Map(ProductEntity product) =>
        new(
            product.Id,
            product.Id,
            product.BoutiqueApplication.BoutiqueName,
            product.Name,
            product.Sku,
            product.Category,
            product.Status,
            product.ReadinessPassed,
            product.TryOnJobs.Count > 0
                ? product.TryOnJobs.OrderByDescending(job => job.CreatedAtUtc)
                    .First()
                    .Status
                : "Not submitted to YouCam",
            product.SubmittedAtUtc,
            product.DecisionNotes,
            product.DecidedAtUtc);
}
