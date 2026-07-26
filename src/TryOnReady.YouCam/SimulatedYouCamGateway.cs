using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using TryOnReady.Application.VirtualTryOn;

namespace TryOnReady.YouCam;

internal sealed class SimulatedYouCamGateway(
    ILogger<SimulatedYouCamGateway> logger)
    : IApparelVirtualTryOnGateway
{
    private readonly ConcurrentDictionary<string, VirtualTryOnResult> results = [];

    public Task<VirtualTryOnSubmission> SubmitAsync(
        VirtualTryOnRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var providerReference = $"simulated-{request.JobId:N}";
        var extension = request.PersonImage.MediaType switch
        {
            "image/jpeg" => ".jpg",
            "image/webp" => ".webp",
            _ => ".png",
        };
        results[providerReference] = new VirtualTryOnResult(
            $"simulated-result-{request.JobId:N}{extension}",
            request.PersonImage.MediaType,
            request.PersonImage.Content);

        logger.LogInformation(
            "Virtual try-on job {JobId} entered deterministic provider simulation.",
            request.JobId);

        return Task.FromResult(
            new VirtualTryOnSubmission(
                VirtualTryOnSubmissionStatus.Accepted,
                providerReference,
                "The simulated provider accepted the task. No API unit was spent.",
                ConsumesApiUnits: false));
    }

    public Task<VirtualTryOnProgress> GetProgressAsync(
        string providerReference,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(
            results.TryGetValue(providerReference, out var result)
                ? new VirtualTryOnProgress(
                    VirtualTryOnProgressStatus.Succeeded,
                    "The deterministic provider simulation completed.",
                    ProviderErrorCode: null,
                    result)
                : new VirtualTryOnProgress(
                    VirtualTryOnProgressStatus.Failed,
                    "The simulated task was not found.",
                    "invalid_simulated_task",
                    Result: null));
    }

    public Task DeleteResourcesAsync(
        string providerReference,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        results.TryRemove(providerReference, out _);
        return Task.CompletedTask;
    }
}
