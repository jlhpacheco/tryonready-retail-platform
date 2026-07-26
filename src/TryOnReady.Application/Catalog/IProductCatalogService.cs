namespace TryOnReady.Application.Catalog;

public interface IProductCatalogService
{
    Task<IReadOnlyList<ProductCatalogItem>> GetProductsAsync(
        string? status,
        CancellationToken cancellationToken);

    Task<ProductCatalogItem?> GetProductAsync(
        Guid productId,
        CancellationToken cancellationToken);

    Task<ProductImageContent?> GetGarmentImageAsync(
        Guid productId,
        CancellationToken cancellationToken);

    Task<ProductCatalogItem> SubmitAsync(
        ProductCatalogSubmission submission,
        CancellationToken cancellationToken);

    Task<ProductCatalogItem?> RecordDecisionAsync(
        Guid productId,
        string decision,
        string? notes,
        CancellationToken cancellationToken);
}

public sealed record ProductCatalogSubmission(
    Guid BoutiqueApplicationId,
    string Name,
    string Sku,
    string Category,
    string Brand,
    string Color,
    string Material,
    string SizeRange,
    string Description,
    decimal? Price,
    string Currency,
    string? ProductUrl,
    string FileName,
    string MediaType,
    int PixelWidth,
    int PixelHeight,
    byte[] Content);

public sealed record ProductCatalogItem(
    Guid Id,
    Guid BoutiqueApplicationId,
    string BoutiqueName,
    string Name,
    string Sku,
    string Category,
    string Brand,
    string Color,
    string Material,
    string SizeRange,
    string Description,
    decimal? Price,
    string Currency,
    string? ProductUrl,
    string Status,
    bool ReadinessPassed,
    string GarmentImageUrl,
    string FileName,
    string MediaType,
    long ByteLength,
    int PixelWidth,
    int PixelHeight,
    DateTimeOffset SubmittedAtUtc,
    string? DecisionNotes,
    DateTimeOffset? DecidedAtUtc);

public sealed record ProductImageContent(
    string FileName,
    string MediaType,
    byte[] Content);
