using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using TryOnReady.Infrastructure.Persistence;
using TryOnReady.Infrastructure.Storage;

namespace TryOnReady.Api.Health;

internal sealed class DatabaseHealthCheck(
    IDbContextFactory<TryOnReadyDbContext> contextFactory) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext =
                await contextFactory.CreateDbContextAsync(cancellationToken);
            return await dbContext.Database.CanConnectAsync(cancellationToken)
                ? HealthCheckResult.Healthy("Database connection succeeded.")
                : HealthCheckResult.Unhealthy("Database connection failed.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy(
                "Database connection failed.",
                exception);
        }
    }
}

internal sealed class PrivateStorageHealthCheck(
    IOptions<PrivateStorageOptions> storageOptions) : IHealthCheck
{
    private const long UnhealthyFreeBytes = 64L * 1024 * 1024;
    private const long DegradedFreeBytes = 128L * 1024 * 1024;

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var fullPath = Path.GetFullPath(storageOptions.Value.RootPath);
            var root = Path.GetPathRoot(fullPath)
                ?? throw new InvalidOperationException(
                    "Private storage path has no filesystem root.");
            var drive = new DriveInfo(root);
            var data = new Dictionary<string, object>
            {
                ["availableBytes"] = drive.AvailableFreeSpace,
                ["totalBytes"] = drive.TotalSize,
            };

            if (drive.AvailableFreeSpace < UnhealthyFreeBytes)
            {
                return Task.FromResult(
                    HealthCheckResult.Unhealthy(
                        "Private storage has less than 64 MB free.",
                        data: data));
            }

            if (drive.AvailableFreeSpace < DegradedFreeBytes)
            {
                return Task.FromResult(
                    HealthCheckResult.Degraded(
                        "Private storage has less than 128 MB free.",
                        data: data));
            }

            return Task.FromResult(
                HealthCheckResult.Healthy(
                    "Private storage capacity is available.",
                    data));
        }
        catch (Exception exception)
        {
            return Task.FromResult(
                HealthCheckResult.Unhealthy(
                    "Private storage capacity could not be checked.",
                    exception));
        }
    }
}

internal sealed class MemoryHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var available = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
        var workingSet = Environment.WorkingSet;
        var data = new Dictionary<string, object>
        {
            ["workingSetBytes"] = workingSet,
            ["availableMemoryBytes"] = available,
        };

        if (available <= 0)
        {
            return Task.FromResult(
                HealthCheckResult.Healthy(
                    "Container memory limit was not reported.",
                    data));
        }

        var ratio = (double)workingSet / available;
        if (ratio >= 0.92)
        {
            return Task.FromResult(
                HealthCheckResult.Unhealthy(
                    "Working set is above 92% of the container memory limit.",
                    data: data));
        }

        if (ratio >= 0.80)
        {
            return Task.FromResult(
                HealthCheckResult.Degraded(
                    "Working set is above 80% of the container memory limit.",
                    data: data));
        }

        return Task.FromResult(
            HealthCheckResult.Healthy(
                "Container memory headroom is available.",
                data));
    }
}
