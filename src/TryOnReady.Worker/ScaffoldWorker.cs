namespace TryOnReady.Worker;

public sealed class ScaffoldWorker(ILogger<ScaffoldWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("TryOnReady scaffold worker started; live virtual try-on processing is disabled.");

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            logger.LogDebug("TryOnReady scaffold worker heartbeat.");
        }
    }
}
