using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TryOnReady.Application.Storage;
using TryOnReady.Application.TryOn;
using TryOnReady.Infrastructure.Processing;
using TryOnReady.Infrastructure.Storage;

namespace TryOnReady.Infrastructure.Persistence;

internal sealed class PersistentTryOnService(
    IDbContextFactory<TryOnReadyDbContext> dbContextFactory,
    IPrivateAssetStore assetStore,
    IOptions<PrivateStorageOptions> storageOptions,
    IOptions<TryOnProcessingOptions> processingOptions)
    : ITryOnService
{
    public async Task<TryOnJobView> SubmitAsync(
        TryOnJobSubmission submission,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(submission);
        if (!submission.ConsentAccepted)
        {
            throw new InvalidOperationException(
                "Consent is required before starting a virtual try-on.");
        }

        var fingerprint = CreateFingerprint(submission);
        await using var dbContext =
            await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var existing = await dbContext.TryOnJobs
            .Include(job => job.Product)
            .SingleOrDefaultAsync(
                job => job.RequestFingerprint == fingerprint,
                cancellationToken);
        if (existing is not null)
        {
            existing.DuplicateRequestCount++;
            existing.UpdatedAtUtc = DateTimeOffset.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
            return Map(existing, isDuplicate: true);
        }

        var product = await dbContext.Products
            .Include(item => item.BoutiqueApplication)
            .SingleOrDefaultAsync(item => item.Id == submission.ProductId, cancellationToken);

        if (product is null
            || product.Status != "Approved"
            || product.BoutiqueApplication.Status != "Approved")
        {
            throw new InvalidOperationException(
                "The boutique and garment must be approved before consumer try-on.");
        }

        var now = DateTimeOffset.UtcNow;
        var personAsset = await assetStore.SaveAsync(
            new PrivateAssetUpload(
                submission.FileName,
                submission.MediaType,
                "consumer-source",
                submission.Content,
                now.AddHours(storageOptions.Value.PersonInputRetentionHours)),
            cancellationToken);

        var entity = new TryOnJobEntity
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Status = "Pending",
            Message = "Secure upload accepted. Waiting for server-side processing.",
            RequestFingerprint = fingerprint,
            PersonAssetId = personAsset.Id,
            PersonMediaType = personAsset.MediaType,
            ConsentVersion = submission.ConsentVersion,
            ConsentAcceptedAtUtc = now,
            ApiUnitsReserved = processingOptions.Value.ApiUnitsPerTask,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
            NextAttemptAtUtc = now,
        };

        try
        {
            dbContext.TryOnJobs.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
            return Map(entity, isDuplicate: false);
        }
        catch (DbUpdateException)
        {
            await assetStore.DeleteAsync(personAsset.Id, CancellationToken.None);
            dbContext.ChangeTracker.Clear();
            var concurrent = await dbContext.TryOnJobs
                .Include(job => job.Product)
                .SingleOrDefaultAsync(
                    job => job.RequestFingerprint == fingerprint,
                    cancellationToken);
            if (concurrent is null)
            {
                throw;
            }

            concurrent.DuplicateRequestCount++;
            concurrent.UpdatedAtUtc = DateTimeOffset.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
            return Map(concurrent, isDuplicate: true);
        }
    }

    public async Task<TryOnJobView?> GetAsync(
        Guid jobId,
        CancellationToken cancellationToken)
    {
        await using var dbContext =
            await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var job = await dbContext.TryOnJobs
            .AsNoTracking()
            .Include(item => item.Product)
            .SingleOrDefaultAsync(item => item.Id == jobId, cancellationToken);
        return job is null ? null : Map(job, isDuplicate: false);
    }

    public async Task<TryOnResultContent?> GetResultAsync(
        Guid jobId,
        CancellationToken cancellationToken)
    {
        await using var dbContext =
            await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var job = await dbContext.TryOnJobs
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == jobId, cancellationToken);
        if (job?.Status != "Succeeded" || job.ResultAssetId is null)
        {
            return null;
        }

        var result = await assetStore.ReadAsync(
            job.ResultAssetId,
            cancellationToken);
        return result is null
            ? null
            : new TryOnResultContent(
                result.Metadata.FileName,
                result.Metadata.MediaType,
                result.Content);
    }

    public async Task<TryOnDashboard> GetDashboardAsync(
        CancellationToken cancellationToken)
    {
        await using var dbContext =
            await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var jobs = await dbContext.TryOnJobs
            .AsNoTracking()
            .Select(job => new
            {
                job.Status,
                job.DuplicateRequestCount,
                job.ApiUnitsReserved,
                job.ApiUnitsConsumed,
            })
            .ToArrayAsync(cancellationToken);

        return new TryOnDashboard(
            jobs.Length,
            jobs.Count(job => job.Status == "Pending"),
            jobs.Count(job => job.Status is "Submitting" or "Processing"),
            jobs.Count(job => job.Status == "Succeeded"),
            jobs.Count(job => job.Status == "Failed"),
            jobs.Sum(job => job.DuplicateRequestCount),
            jobs.Sum(job => job.ApiUnitsReserved),
            jobs.Sum(job => job.ApiUnitsConsumed));
    }

    private static string CreateFingerprint(TryOnJobSubmission submission)
    {
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        hash.AppendData(submission.ProductId.ToByteArray());
        hash.AppendData(Encoding.UTF8.GetBytes(submission.ConsentVersion));
        hash.AppendData(submission.Content);
        return Convert.ToHexString(hash.GetHashAndReset());
    }

    internal static TryOnJobView Map(
        TryOnJobEntity job,
        bool isDuplicate) =>
        new(
            job.Id,
            job.ProductId,
            job.Product.Name,
            job.Status,
            job.Message,
            isDuplicate,
            job.DuplicateRequestCount,
            job.ApiUnitsReserved,
            job.ApiUnitsConsumed,
            job.ResultAssetId is null
                ? null
                : $"/api/try-on-jobs/{job.Id}/result",
            job.CreatedAtUtc,
            job.UpdatedAtUtc,
            job.CompletedAtUtc);
}

