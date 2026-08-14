using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TryOnReady.Application.VirtualTryOn;

namespace TryOnReady.YouCam;

internal sealed class SimulatedYouCamGateway(
    IOptions<YouCamOptions> options,
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
        var resultContent = request.PersonImage.Content;
        var resultMediaType = request.PersonImage.MediaType;
        var extension = resultMediaType switch
        {
            "image/jpeg" => ".jpg",
            "image/webp" => ".webp",
            _ => ".png",
        };

        var configuredResultPath = options.Value.SimulationResultPath;
        if (!string.IsNullOrWhiteSpace(configuredResultPath)
            && File.Exists(configuredResultPath))
        {
            resultContent = File.ReadAllBytes(configuredResultPath);
            extension = Path.GetExtension(configuredResultPath).ToLowerInvariant();
            resultMediaType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".webp" => "image/webp",
                _ => "image/png",
            };
        }

        results[providerReference] = new VirtualTryOnResult(
            $"simulated-result-{request.JobId:N}{extension}",
            resultMediaType,
            resultContent);

        logger.LogInformation(
            "Virtual try-on job {JobId} selected the stored controlled demonstration replay. No provider request was made.",
            request.JobId);

        return Task.FromResult(
            new VirtualTryOnSubmission(
                VirtualTryOnSubmissionStatus.Accepted,
                providerReference,
                "Stored replay selected. Playback makes zero new provider requests.",
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
                    "The previously completed controlled demonstration is ready. Playback made zero new provider requests.",
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
