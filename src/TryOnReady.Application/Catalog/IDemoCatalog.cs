using TryOnReady.Domain.Boutiques;
using TryOnReady.Domain.Products;

namespace TryOnReady.Application.Catalog;

public interface IDemoCatalog
{
    Task<Boutique> GetBoutiqueAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Product>> GetProductsAsync(CancellationToken cancellationToken);
}
