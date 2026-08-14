using Microsoft.Extensions.Logging;
using TryOnReady.Application.VirtualTryOn;

namespace TryOnReady.YouCam;

internal sealed class WindowedYouCamGateway(
    YouCamProviderModeResolver providerMode,
    YouCamApparelVirtualTryOnGateway liveGateway,
    SimulatedYouCamGateway replayGateway,
    ILogger<WindowedYouCamGateway> logger)
    : IApparelVirtualTryOnGateway
{
    public Task<VirtualTryOnSubmission> SubmitAsync(
        VirtualTryOnRequest request,
        CancellationToken cancellationToken) =>
        providerMode.Current == YouCamProviderMode.YouCamLive
            ? liveGateway.SubmitAsync(request, cancellationToken)
            : replayGateway.SubmitAsync(request, cancellationToken);

    public Task<VirtualTryOnProgress> GetProgressAsync(
        string providerReference,
        CancellationToken cancellationToken)
    {
        if (IsReplayReference(providerReference))
        {
            return replayGateway.GetProgressAsync(
                providerReference,
                cancellationToken);
        }

        if (providerMode.Current == YouCamProviderMode.YouCamLive)
        {
            return liveGateway.GetProgressAsync(
                providerReference,
                cancellationToken);
        }

        logger.LogWarning(
            "A YouCam task poll was blocked because the configured live window is closed.");
        return Task.FromResult(
            new VirtualTryOnProgress(
                VirtualTryOnProgressStatus.Failed,
                "The controlled YouCam live window is closed.",
                "live_window_closed",
                Result: null));
    }

    public Task DeleteResourcesAsync(
        string providerReference,
        CancellationToken cancellationToken)
    {
        if (IsReplayReference(providerReference))
        {
            return replayGateway.DeleteResourcesAsync(
                providerReference,
                cancellationToken);
        }

        if (providerMode.Current == YouCamProviderMode.YouCamLive)
        {
            return liveGateway.DeleteResourcesAsync(
                providerReference,
                cancellationToken);
        }

        logger.LogInformation(
            "A YouCam cleanup call was skipped because the configured live window is closed.");
        return Task.CompletedTask;
    }

    private static bool IsReplayReference(string providerReference) =>
        providerReference.StartsWith("simulated-", StringComparison.Ordinal);
}
