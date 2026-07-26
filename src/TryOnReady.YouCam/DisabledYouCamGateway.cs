using Microsoft.Extensions.Logging;
using TryOnReady.Application.VirtualTryOn;

namespace TryOnReady.YouCam;

internal sealed class DisabledYouCamGateway(
    ILogger<DisabledYouCamGateway> logger) : IApparelVirtualTryOnGateway
{
    public Task<VirtualTryOnSubmission> SubmitAsync(
        VirtualTryOnRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        logger.LogInformation(
            "Virtual try-on job {JobId} was not submitted because the scaffold adapter is disabled.",
            request.JobId);

        return Task.FromResult(
            new VirtualTryOnSubmission(
                VirtualTryOnSubmissionStatus.Disabled,
                ProviderReference: null,
                "Live YouCam processing is disabled.",
                ConsumesApiUnits: false));
    }

    public Task<VirtualTryOnProgress> GetProgressAsync(
        string providerReference,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(
            new VirtualTryOnProgress(
                VirtualTryOnProgressStatus.Failed,
                "Live YouCam processing is disabled.",
                "provider_disabled",
                Result: null));
    }

    public Task DeleteResourcesAsync(
        string providerReference,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}
