using TryOnReady.Domain.Products;
using TryOnReady.Domain.Readiness;

namespace TryOnReady.Application.Readiness;

public interface IProductReadinessService
{
    ProductReadinessResult Assess(ProductImageMetadata image);
}
