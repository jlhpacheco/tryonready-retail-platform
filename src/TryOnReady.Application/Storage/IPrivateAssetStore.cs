namespace TryOnReady.Application.Storage;

public interface IPrivateAssetStore
{
    Task<StoredPrivateAsset> SaveAsync(
        PrivateAssetUpload upload,
        CancellationToken cancellationToken);

    Task<PrivateAssetContent?> ReadAsync(
        string assetId,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        string assetId,
        CancellationToken cancellationToken);

    Task<int> DeleteExpiredAsync(
        DateTimeOffset utcNow,
        CancellationToken cancellationToken);
}

public sealed record PrivateAssetUpload(
    string FileName,
    string MediaType,
    string Purpose,
    byte[] Content,
    DateTimeOffset? ExpiresAtUtc);

public sealed record StoredPrivateAsset(
    string Id,
    string FileName,
    string MediaType,
    long ByteLength,
    string Sha256,
    string Purpose,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? ExpiresAtUtc);

public sealed record PrivateAssetContent(
    StoredPrivateAsset Metadata,
    byte[] Content);
