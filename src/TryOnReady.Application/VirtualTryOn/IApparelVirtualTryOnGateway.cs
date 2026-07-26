namespace TryOnReady.Application.VirtualTryOn;

public interface IApparelVirtualTryOnGateway
{
    Task<VirtualTryOnSubmission> SubmitAsync(
        VirtualTryOnRequest request,
        CancellationToken cancellationToken);
}

public sealed record VirtualTryOnRequest(
    Guid JobId,
    Guid ProductId,
    string GarmentAssetReference);

public sealed record VirtualTryOnSubmission(
    VirtualTryOnSubmissionStatus Status,
    string? ProviderReference,
    string Message);

public enum VirtualTryOnSubmissionStatus
{
    Disabled,
    Accepted,
    Rejected,
}
