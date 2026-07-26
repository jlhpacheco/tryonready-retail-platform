using TryOnReady.Api.Contracts;
using TryOnReady.Application.Catalog;
using TryOnReady.Application.Readiness;
using TryOnReady.Domain.Products;

namespace TryOnReady.Api.Endpoints;

internal static class ScaffoldEndpoints
{
    public static IEndpointRouteBuilder MapScaffoldEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
                "/api",
                () => TypedResults.Ok(
                    new
                    {
                        name = "TryOnReady API",
                        phase = "scaffold",
                        liveYouCamIntegration = false,
                        health = "/health",
                        openApi = "/openapi/v1.json",
                        scaffoldStatus = "/api/scaffold",
                        syntheticCatalog = "/api/demo/catalog",
                        readinessAssessment = "/api/readiness/assess",
                    }))
            .WithName("GetApiEntryPoint")
            .WithTags("Scaffold");

        endpoints.MapGet(
                "/api/scaffold",
                () => TypedResults.Ok(
                    new
                    {
                        name = "TryOnReady API",
                        phase = "scaffold",
                        liveYouCamIntegration = false,
                    }))
            .WithName("GetScaffoldStatus")
            .WithTags("Scaffold");

        endpoints.MapGet(
                "/api/demo/catalog",
                async (IDemoCatalog catalog, CancellationToken cancellationToken) =>
                {
                    var boutique = await catalog.GetBoutiqueAsync(cancellationToken);
                    var products = await catalog.GetProductsAsync(cancellationToken);
                    return TypedResults.Ok(new { boutique, products });
                })
            .WithName("GetSyntheticDemoCatalog")
            .WithTags("Demo");

        endpoints.MapPost(
                "/api/readiness/assess",
                (ProductReadinessRequest request, IProductReadinessService readinessService) =>
                {
                    var errors = Validate(request);
                    if (errors.Count > 0)
                    {
                        return Results.ValidationProblem(errors);
                    }

                    var image = new ProductImageMetadata(
                        request.ProductId,
                        request.FileName!,
                        request.MediaType!,
                        request.ByteLength,
                        request.PixelWidth,
                        request.PixelHeight);

                    return Results.Ok(readinessService.Assess(image));
                })
            .WithName("AssessProductReadiness")
            .WithTags("Readiness");

        return endpoints;
    }

    private static Dictionary<string, string[]> Validate(ProductReadinessRequest request)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);

        if (request.ProductId == Guid.Empty)
        {
            errors[nameof(request.ProductId)] = ["A product identifier is required."];
        }

        if (string.IsNullOrWhiteSpace(request.FileName))
        {
            errors[nameof(request.FileName)] = ["A file name is required."];
        }

        if (string.IsNullOrWhiteSpace(request.MediaType))
        {
            errors[nameof(request.MediaType)] = ["A media type is required."];
        }

        return errors;
    }
}
