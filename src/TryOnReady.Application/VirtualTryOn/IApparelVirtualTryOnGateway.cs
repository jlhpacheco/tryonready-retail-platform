namespace TryOnReady.Application.VirtualTryOn;

public interface IApparelVirtualTryOnGateway
{
    Task<VirtualTryOnSubmission> SubmitAsync(
        VirtualTryOnRequest request,
        CancellationToken cancellationToken);

    Task<VirtualTryOnProgress> GetProgressAsync(
        string providerReference,
        CancellationToken cancellationToken);

    Task DeleteResourcesAsync(
        string providerReference,
        CancellationToken cancellationToken);
}

public sealed record VirtualTryOnRequest(
    Guid JobId,
    Guid ProductId,
    string GarmentCategory,
    VirtualTryOnAsset PersonImage,
    VirtualTryOnAsset GarmentImage);

public sealed record VirtualTryOnAsset(
    string FileName,
    string MediaType,
    byte[] Content);

public sealed record VirtualTryOnSubmission(
    VirtualTryOnSubmissionStatus Status,
    string? ProviderReference,
    string Message,
    bool ConsumesApiUnits);

public sealed record VirtualTryOnProgress(
    VirtualTryOnProgressStatus Status,
    string Message,
    string? ProviderErrorCode,
    VirtualTryOnResult? Result);

public sealed record VirtualTryOnResult(
    string FileName,
    string MediaType,
    byte[] Content);

public enum VirtualTryOnSubmissionStatus
{
    Disabled,
    Accepted,
    Rejected,
}

public enum VirtualTryOnProgressStatus
{
    Pending,
    Running,
    Succeeded,
    Failed,
}
