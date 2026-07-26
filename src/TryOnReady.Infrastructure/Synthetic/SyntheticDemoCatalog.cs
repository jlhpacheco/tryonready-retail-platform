using TryOnReady.Application.Catalog;
using TryOnReady.Domain.Boutiques;
using TryOnReady.Domain.Products;

namespace TryOnReady.Infrastructure.Synthetic;

public sealed class SyntheticDemoCatalog : IDemoCatalog
{
    private static readonly Boutique Boutique = new(
        Guid.Parse("15d2ef07-3664-4027-849f-ed47cb1cb835"),
        "Luna & Thread",
        "Elena Rivera",
        3);

    private static readonly IReadOnlyList<Product> Products =
    [
        new(
            Guid.Parse("bac012b4-fc08-4f74-a587-4b42fb791906"),
            Boutique.Id,
            "Moonlight Blazer",
            "SYN-BLZ-001",
            ProductStatus.ReadinessReview),
    ];

    public Task<Boutique> GetBoutiqueAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Boutique);
    }

    public Task<IReadOnlyList<Product>> GetProductsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Products);
    }
}
