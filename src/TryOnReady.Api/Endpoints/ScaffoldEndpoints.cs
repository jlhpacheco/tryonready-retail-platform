using TryOnReady.Api.Contracts;
using TryOnReady.Application.AdminReview;
using TryOnReady.Application.BoutiqueApplications;
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
                        adminReviews = "/api/admin/reviews",
                        boutiqueApplications = "/api/boutique-applications",
                        consumerTryOnPreflight = "/api/consumer/try-on/preflight",
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

        endpoints.MapGet(
                "/api/boutique-applications",
                async (
                    IBoutiqueApplicationService applicationService,
                    CancellationToken cancellationToken) =>
                    TypedResults.Ok(
                        await applicationService.GetApplicationsAsync(
                            cancellationToken)))
            .WithName("GetBoutiqueApplications")
            .WithTags("Boutique Applications");

        endpoints.MapPost(
                "/api/boutique-applications",
                async (
                    BoutiqueApplicationRequest request,
                    IBoutiqueApplicationService applicationService,
                    CancellationToken cancellationToken) =>
                {
                    var errors = Validate(request);
                    if (errors.Count > 0)
                    {
                        return Results.ValidationProblem(errors);
                    }

                    var application = await applicationService.SubmitAsync(
                        request.BoutiqueName!,
                        request.OwnerName!,
                        request.Email!,
                        request.EmployeeCount,
                        request.PrimarySalesChannel!,
                        request.Website,
                        cancellationToken);

                    return Results.Ok(application);
                })
            .WithName("SubmitBoutiqueApplication")
            .WithTags("Boutique Applications");

        endpoints.MapPost(
                "/api/boutique-applications/{applicationId:guid}/decision",
                async (
                    Guid applicationId,
                    BoutiqueApplicationDecisionRequest request,
                    IBoutiqueApplicationService applicationService,
                    CancellationToken cancellationToken) =>
                {
                    var decision = NormalizeApplicationDecision(request.Decision);
                    if (decision is null)
                    {
                        return Results.ValidationProblem(
                            new Dictionary<string, string[]>
                            {
                                [nameof(request.Decision)] =
                                [
                                    "Choose Approve or Decline.",
                                ],
                            });
                    }

                    var application = await applicationService.RecordDecisionAsync(
                        applicationId,
                        decision,
                        cancellationToken);

                    return application is null
                        ? Results.NotFound()
                        : Results.Ok(application);
                })
            .WithName("RecordBoutiqueApplicationDecision")
            .WithTags("Boutique Applications");

        endpoints.MapGet(
                "/api/admin/reviews",
                async (
                    IAdminReviewService reviewService,
                    CancellationToken cancellationToken) =>
                    TypedResults.Ok(
                        await reviewService.GetReviewsAsync(cancellationToken)))
            .WithName("GetAdminReviews")
            .WithTags("Admin Review");

        endpoints.MapPost(
                "/api/admin/reviews/{reviewId:guid}/decision",
                async (
                    Guid reviewId,
                    AdminReviewDecisionRequest request,
                    IAdminReviewService reviewService,
                    CancellationToken cancellationToken) =>
                {
                    var decision = NormalizeDecision(request.Decision);

                    if (decision is null)
                    {
                        return Results.ValidationProblem(
                            new Dictionary<string, string[]>
                            {
                                [nameof(request.Decision)] =
                                [
                                    "Choose Approve, ChangesRequested, or Decline.",
                                ],
                            });
                    }

                    if (decision is "ChangesRequested" or "Declined"
                        && string.IsNullOrWhiteSpace(request.Notes))
                    {
                        return Results.ValidationProblem(
                            new Dictionary<string, string[]>
                            {
                                [nameof(request.Notes)] =
                                [
                                    "Add a clear note when requesting changes or declining.",
                                ],
                            });
                    }

                    var review = await reviewService.RecordDecisionAsync(
                        reviewId,
                        decision,
                        request.Notes,
                        cancellationToken);

                    return review is null
                        ? Results.NotFound()
                        : Results.Ok(review);
                })
            .WithName("RecordAdminReviewDecision")
            .WithTags("Admin Review");

        endpoints.MapPost(
                "/api/consumer/try-on/preflight",
                async (
                    ConsumerTryOnPreflightRequest request,
                    IAdminReviewService reviewService,
                    IProductReadinessService readinessService,
                    CancellationToken cancellationToken) =>
                {
                    var reviews = await reviewService.GetReviewsAsync(cancellationToken);
                    var approvedProduct = reviews.FirstOrDefault(
                        review => review.ProductId == request.ProductId
                            && review.Status == "Approved");

                    if (approvedProduct is null)
                    {
                        return Results.ValidationProblem(
                            new Dictionary<string, string[]>
                            {
                                [nameof(request.ProductId)] =
                                [
                                    "The product must be approved before consumer try-on.",
                                ],
                            });
                    }

                    if (!request.ConsentAccepted)
                    {
                        return Results.ValidationProblem(
                            new Dictionary<string, string[]>
                            {
                                [nameof(request.ConsentAccepted)] =
                                [
                                    "Consent is required before preparing a try-on.",
                                ],
                            });
                    }

                    if (string.IsNullOrWhiteSpace(request.FileName)
                        || string.IsNullOrWhiteSpace(request.MediaType))
                    {
                        return Results.ValidationProblem(
                            new Dictionary<string, string[]>
                            {
                                [nameof(request.FileName)] =
                                [
                                    "Choose an authorized synthetic person image.",
                                ],
                            });
                    }

                    var assessment = readinessService.Assess(
                        new ProductImageMetadata(
                            request.ProductId,
                            request.FileName,
                            request.MediaType,
                            request.ByteLength,
                            request.PixelWidth,
                            request.PixelHeight));

                    if (!assessment.IsReady)
                    {
                        return Results.BadRequest(assessment);
                    }

                    return Results.Ok(
                        new
                        {
                            sessionId = Guid.NewGuid(),
                            status = "ReadyForProvider",
                            productName = approvedProduct.ProductName,
                            providerEnabled = false,
                            message =
                                "Preflight passed. Live YouCam generation is not enabled yet.",
                        });
                })
            .WithName("PrepareConsumerTryOn")
            .WithTags("Consumer Try-On");

        return endpoints;
    }

    private static Dictionary<string, string[]> Validate(
        BoutiqueApplicationRequest request)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);

        if (string.IsNullOrWhiteSpace(request.BoutiqueName))
        {
            errors[nameof(request.BoutiqueName)] = ["Boutique name is required."];
        }

        if (string.IsNullOrWhiteSpace(request.OwnerName))
        {
            errors[nameof(request.OwnerName)] = ["Owner name is required."];
        }

        if (string.IsNullOrWhiteSpace(request.Email)
            || !request.Email.Contains('@', StringComparison.Ordinal))
        {
            errors[nameof(request.Email)] = ["Enter a valid email address."];
        }

        if (request.EmployeeCount is < 1 or > 250)
        {
            errors[nameof(request.EmployeeCount)] =
                ["Employee count must be between 1 and 250."];
        }

        if (string.IsNullOrWhiteSpace(request.PrimarySalesChannel))
        {
            errors[nameof(request.PrimarySalesChannel)] =
                ["Choose a primary sales channel."];
        }

        if (!request.CertifiesImageRights)
        {
            errors[nameof(request.CertifiesImageRights)] =
                ["Confirm that the boutique will use authorized images."];
        }

        return errors;
    }

    private static string? NormalizeDecision(string? decision) =>
        decision?.Trim().ToUpperInvariant() switch
        {
            "APPROVE" or "APPROVED" => "Approved",
            "CHANGESREQUESTED" or "REQUESTCHANGES" => "ChangesRequested",
            "DECLINE" or "DECLINED" => "Declined",
            _ => null,
        };

    private static string? NormalizeApplicationDecision(string? decision) =>
        decision?.Trim().ToUpperInvariant() switch
        {
            "APPROVE" or "APPROVED" => "Approved",
            "DECLINE" or "DECLINED" => "Declined",
            _ => null,
        };

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
