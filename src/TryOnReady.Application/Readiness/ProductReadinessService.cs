using TryOnReady.Domain.Products;
using TryOnReady.Domain.Readiness;

namespace TryOnReady.Application.Readiness;

public sealed class ProductReadinessService : IProductReadinessService
{
    private const long MaximumBytes = 15 * 1024 * 1024;
    private const int MinimumDimension = 1_024;

    private static readonly HashSet<string> SupportedMediaTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/png",
            "image/webp",
        };

    public ProductReadinessResult Assess(ProductImageMetadata image)
    {
        ArgumentNullException.ThrowIfNull(image);

        var issues = new List<ReadinessIssue>();

        if (!SupportedMediaTypes.Contains(image.MediaType))
        {
            issues.Add(new(
                "unsupported_media_type",
                "Use a JPEG, PNG, or WebP garment image.",
                ReadinessSeverity.Blocking));
        }

        if (image.ByteLength <= 0 || image.ByteLength > MaximumBytes)
        {
            issues.Add(new(
                "invalid_file_size",
                "The garment image must be larger than zero bytes and no more than 15 MB.",
                ReadinessSeverity.Blocking));
        }

        if (image.PixelWidth < MinimumDimension || image.PixelHeight < MinimumDimension)
        {
            issues.Add(new(
                "insufficient_resolution",
                "Use an image at least 1024 pixels wide and 1024 pixels tall.",
                ReadinessSeverity.Blocking));
        }

        return issues.Count == 0
            ? ProductReadinessResult.Ready
            : new ProductReadinessResult(false, issues);
    }
}
