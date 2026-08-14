using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TryOnReady.Application.Storage;
using TryOnReady.Application.VirtualTryOn;
using TryOnReady.Infrastructure.Persistence;
using TryOnReady.Infrastructure.Storage;

namespace TryOnReady.Infrastructure.Processing;

internal sealed class TryOnJobProcessor(
    IDbContextFactory<TryOnReadyDbContext> dbContextFactory,
    IPrivateAssetStore assetStore,
    IApparelVirtualTryOnGateway gateway,
    IOptions<TryOnProcessingOptions> processingOptions,
    IOptions<PrivateStorageOptions> storageOptions,
    ILogger<TryOnJobProcessor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var processed = await ProcessNextAsync(stoppingToken);
                if (!processed)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "The virtual try-on processor encountered an unexpected error.");
                await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
            }
        }
    }

    private async Task<bool> ProcessNextAsync(CancellationToken cancellationToken)
    {
        TryOnJobEntity? snapshot;
        await using (var dbContext =
            await dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var now = DateTimeOffset.UtcNow;
            snapshot = await dbContext.TryOnJobs
                .AsNoTracking()
                .Include(job => job.Product)
                .Where(job =>
                    (job.Status == "Pending"
                        || job.Status == "Processing"
                        || job.Status == "Submitting")
                    && (job.NextAttemptAtUtc == null || job.NextAttemptAtUtc <= now))
                .OrderBy(job => job.CreatedAtUtc)
                .FirstOrDefaultAsync(cancellationToken);
        }

        if (snapshot is null)
        {
            return false;
        }

        if (snapshot.AttemptCount >= processingOptions.Value.MaxAttempts)
        {
            await MarkFailedAsync(
                snapshot.Id,
                "provider_poll_timeout",
                "YouCam is taking longer than expected. Please try again in a moment.",
                cancellationToken);
            return true;
        }

        if (snapshot.Status == "Submitting")
        {
            await MarkFailedAsync(
                snapshot.Id,
                "submission_interrupted",
                "The YouCam run was interrupted. Start a new try-on only if you want to try again.",
                cancellationToken);
        }
        else if (snapshot.Status == "Pending")
        {
            await SubmitAsync(snapshot, cancellationToken);
        }
        else
        {
            await PollAsync(snapshot, cancellationToken);
        }

        return true;
    }

    private async Task SubmitAsync(
        TryOnJobEntity snapshot,
        CancellationToken cancellationToken)
    {
        await UpdateStatusAsync(
            snapshot.Id,
            "Submitting",
            "Sending the approved photos to YouCam.",
            providerReference: null,
            providerErrorCode: null,
            consumeUnits: false,
            cancellationToken);

        var person = await assetStore.ReadAsync(
            snapshot.PersonAssetId,
            cancellationToken);
        var garment = await assetStore.ReadAsync(
            snapshot.Product.GarmentAssetId,
            cancellationToken);
        if (person is null || garment is null)
        {
            await MarkFailedAsync(
                snapshot.Id,
                "private_asset_missing",
                "One of the approved photos could not be found. Please try again.",
                cancellationToken);
            return;
        }

        var submission = await gateway.SubmitAsync(
            new VirtualTryOnRequest(
                snapshot.Id,
                snapshot.ProductId,
                snapshot.Product.Category,
                new VirtualTryOnAsset(
                    person.Metadata.FileName,
                    person.Metadata.MediaType,
                    person.Content),
                new VirtualTryOnAsset(
                    garment.Metadata.FileName,
                    garment.Metadata.MediaType,
                    garment.Content)),
            cancellationToken);

        if (submission.Status != VirtualTryOnSubmissionStatus.Accepted
            || string.IsNullOrWhiteSpace(submission.ProviderReference))
        {
            await MarkFailedAsync(
                snapshot.Id,
                submission.Status == VirtualTryOnSubmissionStatus.Disabled
                    ? "provider_disabled"
                    : "provider_rejected",
                submission.Message,
                cancellationToken);
            return;
        }

        await UpdateStatusAsync(
            snapshot.Id,
            "Processing",
            submission.Message,
            submission.ProviderReference,
            providerErrorCode: null,
            consumeUnits: submission.ConsumesApiUnits,
            cancellationToken);
    }

    private async Task PollAsync(
        TryOnJobEntity snapshot,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(snapshot.ProviderReference))
        {
            await MarkFailedAsync(
                snapshot.Id,
                "provider_reference_missing",
                "The YouCam try-on could not be located. Please try again.",
                cancellationToken);
            return;
        }

        var progress = await gateway.GetProgressAsync(
            snapshot.ProviderReference,
            cancellationToken);

        if (progress.Status is VirtualTryOnProgressStatus.Pending
            or VirtualTryOnProgressStatus.Running)
        {
            await UpdateStatusAsync(
                snapshot.Id,
                "Processing",
                progress.Message,
                snapshot.ProviderReference,
                progress.ProviderErrorCode,
                consumeUnits: false,
                cancellationToken);
            return;
        }

        if (progress.Status == VirtualTryOnProgressStatus.Failed
            || progress.Result is null)
        {
            await MarkFailedAsync(
                snapshot.Id,
                progress.ProviderErrorCode ?? "provider_failed",
                progress.Message,
                cancellationToken);
            await DeleteProviderResourcesSafelyAsync(
                snapshot.ProviderReference,
                cancellationToken);
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var result = await assetStore.SaveAsync(
            new PrivateAssetUpload(
                progress.Result.FileName,
                progress.Result.MediaType,
                "generated-result",
                progress.Result.Content,
                now.AddHours(storageOptions.Value.ResultRetentionHours)),
            cancellationToken);

        await using (var dbContext =
            await dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var job = await dbContext.TryOnJobs.FindAsync(
                [snapshot.Id],
                cancellationToken);
            if (job is not null)
            {
                job.Status = "Succeeded";
                job.Message = "Your generated virtual try-on is ready.";
                job.ResultAssetId = result.Id;
                job.ProviderErrorCode = null;
                job.UpdatedAtUtc = now;
                job.CompletedAtUtc = now;
                job.NextAttemptAtUtc = null;
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        await assetStore.DeleteAsync(snapshot.PersonAssetId, cancellationToken);
        await DeleteProviderResourcesSafelyAsync(
            snapshot.ProviderReference,
            cancellationToken);
        logger.LogInformation(
            "Virtual try-on job {JobId} completed successfully.",
            snapshot.Id);
    }

    private async Task UpdateStatusAsync(
        Guid jobId,
        string status,
        string message,
        string? providerReference,
        string? providerErrorCode,
        bool consumeUnits,
        CancellationToken cancellationToken)
    {
        await using var dbContext =
            await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var job = await dbContext.TryOnJobs.FindAsync([jobId], cancellationToken);
        if (job is null)
        {
            return;
        }

        job.Status = status;
        job.Message = message;
        job.ProviderReference = providerReference ?? job.ProviderReference;
        job.ProviderErrorCode = providerErrorCode;
        job.ApiUnitsConsumed = consumeUnits
            ? job.ApiUnitsReserved
            : job.ApiUnitsConsumed;
        job.AttemptCount++;
        job.UpdatedAtUtc = DateTimeOffset.UtcNow;
        job.NextAttemptAtUtc = DateTimeOffset.UtcNow.AddSeconds(
            processingOptions.Value.PollIntervalSeconds);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task MarkFailedAsync(
        Guid jobId,
        string providerErrorCode,
        string message,
        CancellationToken cancellationToken)
    {
        string? personAssetId = null;
        await using (var dbContext =
            await dbContextFactory.CreateDbContextAsync(cancellationToken))
        {
            var job = await dbContext.TryOnJobs.FindAsync([jobId], cancellationToken);
            if (job is null)
            {
                return;
            }

            personAssetId = job.PersonAssetId;
            job.Status = "Failed";
            job.Message = message;
            job.ProviderErrorCode = providerErrorCode;
            job.UpdatedAtUtc = DateTimeOffset.UtcNow;
            job.CompletedAtUtc = DateTimeOffset.UtcNow;
            job.NextAttemptAtUtc = null;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (personAssetId is not null)
        {
            await assetStore.DeleteAsync(personAssetId, cancellationToken);
        }

        logger.LogWarning(
            "Virtual try-on job {JobId} failed with provider code {ProviderErrorCode}.",
            jobId,
            providerErrorCode);
    }

    private async Task DeleteProviderResourcesSafelyAsync(
        string providerReference,
        CancellationToken cancellationToken)
    {
        try
        {
            await gateway.DeleteResourcesAsync(
                providerReference,
                cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                exception,
                "YouCam cleanup for a completed task could not finish immediately.");
        }
    }
}
