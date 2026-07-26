using TryOnReady.Application.Readiness;
using TryOnReady.Domain.Products;

namespace TryOnReady.UnitTests;

public sealed class ProductReadinessServiceTests
{
    private readonly ProductReadinessService service = new();

    [Fact]
    public void Assess_ReturnsReady_ForSupportedImageMetadata()
    {
        var image = new ProductImageMetadata(
            Guid.NewGuid(),
            "synthetic-blazer.jpg",
            "image/jpeg",
            2_000_000,
            1_600,
            1_600);

        var result = service.Assess(image);

        Assert.True(result.IsReady);
        Assert.Empty(result.Issues);
    }

    [Fact]
    public void Assess_ReturnsBlockingIssues_ForUnsupportedLowResolutionImage()
    {
        var image = new ProductImageMetadata(
            Guid.NewGuid(),
            "synthetic-blazer.gif",
            "image/gif",
            500_000,
            800,
            800);

        var result = service.Assess(image);

        Assert.False(result.IsReady);
        Assert.Equal(2, result.Issues.Count);
        Assert.Contains(result.Issues, issue => issue.Code == "unsupported_media_type");
        Assert.Contains(result.Issues, issue => issue.Code == "insufficient_resolution");
    }
}
