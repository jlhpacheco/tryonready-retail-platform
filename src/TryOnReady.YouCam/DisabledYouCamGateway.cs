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
                "Live YouCam requests are not implemented in the scaffold."));
    }
}
