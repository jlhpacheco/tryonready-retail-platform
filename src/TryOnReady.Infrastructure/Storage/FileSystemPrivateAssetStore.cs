using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TryOnReady.Application.Storage;

namespace TryOnReady.Infrastructure.Storage;

internal sealed class FileSystemPrivateAssetStore
    : IPrivateAssetStore
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    private readonly string rootPath;

    public FileSystemPrivateAssetStore(
        IOptions<PrivateStorageOptions> options,
        IHostEnvironment hostEnvironment)
    {
        var configuredPath = options.Value.RootPath;
        rootPath = Path.GetFullPath(
            Path.IsPathRooted(configuredPath)
                ? configuredPath
                : Path.Combine(hostEnvironment.ContentRootPath, configuredPath));
        Directory.CreateDirectory(rootPath);
    }

    public async Task<StoredPrivateAsset> SaveAsync(
        PrivateAssetUpload upload,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(upload);
        cancellationToken.ThrowIfCancellationRequested();

        var id = Guid.NewGuid().ToString("N");
        var metadata = new StoredPrivateAsset(
            id,
            Path.GetFileName(upload.FileName),
            upload.MediaType,
            upload.Content.LongLength,
            Convert.ToHexString(SHA256.HashData(upload.Content)),
            upload.Purpose,
            DateTimeOffset.UtcNow,
            upload.ExpiresAtUtc);

        await File.WriteAllBytesAsync(
            GetContentPath(id),
            upload.Content,
            cancellationToken);
        await File.WriteAllTextAsync(
            GetMetadataPath(id),
            JsonSerializer.Serialize(metadata, JsonOptions),
            cancellationToken);
        return metadata;
    }

    public async Task<PrivateAssetContent?> ReadAsync(
        string assetId,
        CancellationToken cancellationToken)
    {
        if (!IsValidAssetId(assetId))
        {
            return null;
        }

        var metadataPath = GetMetadataPath(assetId);
        var contentPath = GetContentPath(assetId);
        if (!File.Exists(metadataPath) || !File.Exists(contentPath))
        {
            return null;
        }

        var metadataJson = await File.ReadAllTextAsync(
            metadataPath,
            cancellationToken);
        var metadata = JsonSerializer.Deserialize<StoredPrivateAsset>(
            metadataJson,
            JsonOptions);
        if (metadata is null)
        {
            return null;
        }

        var content = await File.ReadAllBytesAsync(contentPath, cancellationToken);
        return new PrivateAssetContent(metadata, content);
    }

    public Task DeleteAsync(
        string assetId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!IsValidAssetId(assetId))
        {
            return Task.CompletedTask;
        }

        File.Delete(GetContentPath(assetId));
        File.Delete(GetMetadataPath(assetId));
        return Task.CompletedTask;
    }

    public async Task<int> DeleteExpiredAsync(
        DateTimeOffset utcNow,
        CancellationToken cancellationToken)
    {
        var deleted = 0;
        foreach (var metadataPath in Directory.EnumerateFiles(rootPath, "*.json"))
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var metadataJson = await File.ReadAllTextAsync(
                    metadataPath,
                    cancellationToken);
                var metadata = JsonSerializer.Deserialize<StoredPrivateAsset>(
                    metadataJson,
                    JsonOptions);
                if (metadata?.ExpiresAtUtc is null
                    || metadata.ExpiresAtUtc > utcNow)
                {
                    continue;
                }

                await DeleteAsync(metadata.Id, cancellationToken);
                deleted++;
            }
            catch (JsonException)
            {
                // Leave unreadable metadata for manual inspection rather than
                // deleting a potentially unrelated file.
            }
        }

        return deleted;
    }

    private string GetContentPath(string assetId) =>
        Path.Combine(rootPath, $"{assetId}.bin");

    private string GetMetadataPath(string assetId) =>
        Path.Combine(rootPath, $"{assetId}.json");

    private static bool IsValidAssetId(string assetId) =>
        assetId.Length == 32
        && assetId.All(character => char.IsAsciiHexDigit(character));
}
