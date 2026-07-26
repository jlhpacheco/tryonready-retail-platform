namespace TryOnReady.Domain.Products;

public sealed record Product(
    Guid Id,
    Guid BoutiqueId,
    string Name,
    string Sku,
    ProductStatus Status);

public enum ProductStatus
{
    Draft,
    ReadinessReview,
    ProviderValidation,
    AdminReview,
    Published,
}
