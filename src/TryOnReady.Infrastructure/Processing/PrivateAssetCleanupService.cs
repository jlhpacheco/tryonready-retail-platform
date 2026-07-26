using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TryOnReady.Application.Storage;

namespace TryOnReady.Infrastructure.Processing;

internal sealed class PrivateAssetCleanupService(
    IPrivateAssetStore assetStore,
    ILogger<PrivateAssetCleanupService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var deleted = await assetStore.DeleteExpiredAsync(
                    DateTimeOffset.UtcNow,
                    stoppingToken);
                if (deleted > 0)
                {
                    logger.LogInformation(
                        "Deleted {AssetCount} expired private assets.",
                        deleted);
                }

                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "The private-asset cleanup pass failed.");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
