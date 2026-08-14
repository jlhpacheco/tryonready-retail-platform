using System.Globalization;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using TryOnReady.Api.Authentication;
using TryOnReady.Api.Security;
using TryOnReady.Application.Catalog;
using TryOnReady.Application.TryOn;
using TryOnReady.Infrastructure.Persistence;
using TryOnReady.YouCam;

namespace TryOnReady.Api.Endpoints;

internal static class WorkflowEndpoints
{
    private const long MaximumImageBytes = 10 * 1024 * 1024;
    private static readonly HashSet<string> SupportedMediaTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/png",
            "image/webp",
        };

    public static IServiceCollection ConfigureWorkflowUploads(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<FormOptions>(
            options =>
            {
                options.MultipartBodyLengthLimit = 22 * 1024 * 1024;
                options.ValueLengthLimit = 2 * 1024 * 1024;
            });
        services.AddOptions<HostedDemoOptions>()
            .Bind(configuration.GetSection(HostedDemoOptions.SectionName))
            .Validate(
                options => !options.SyntheticOnly ||
                    IsSha256(options.AllowedGarmentImageSha256),
                "HostedDemo:AllowedGarmentImageSha256 must be a SHA-256 hash when synthetic-only mode is enabled.")
            .Validate(
                options => !options.SyntheticOnly ||
                    IsSha256(options.AllowedPersonImageSha256),
                "HostedDemo:AllowedPersonImageSha256 must be a SHA-256 hash when synthetic-only mode is enabled.")
            .ValidateOnStart();
        return services;
    }

    public static IEndpointRouteBuilder MapWorkflowEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
                "/api/status",
                (
                    IOptions<YouCamOptions> youCam,
                    YouCamProviderModeResolver providerMode,
                    IOptions<PersistenceOptions> persistence,
                    IOptions<HostedDemoOptions> hostedDemo) =>
                    TypedResults.Ok(
                        new
                        {
                            phase = "vertical-slice",
                            providerMode = providerMode.Current.ToString(),
                            liveYouCamIntegration = providerMode.Current
                                == YouCamProviderMode.YouCamLive,
                            youCamApi =
                                "YouCam AI Clothes v3 / Apparel Virtual Try-On",
                            demonstrationLabel = providerMode.Current
                                == YouCamProviderMode.StoredReplay
                                ? "previously completed controlled demonstration"
                                : null,
                            playbackMakesNewProviderRequests = false,
                            automaticLiveWindowEnabled =
                                youCam.Value.AutomaticLiveWindowEnabled,
                            liveWindowStartsAtUtc =
                                youCam.Value.LiveWindowStartsAtUtc,
                            liveWindowEndsAtUtc =
                                youCam.Value.LiveWindowEndsAtUtc,
                            syntheticOnlyJudgeDemo =
                                hostedDemo.Value.SyntheticOnly,
                            persistence = persistence.Value.Provider,
                            apiKeyExposedToBrowser = false,
                        }))
            .WithName("GetWorkflowStatus")
            .WithTags("Status");

        endpoints.MapGet(
                "/api/products",
                async (
                    string? status,
                    IProductCatalogService catalog,
                    CancellationToken cancellationToken) =>
                    TypedResults.Ok(
                        await catalog.GetProductsAsync(status, cancellationToken)))
            .WithName("GetProducts")
            .WithTags("Catalog");

        endpoints.MapPost(
                "/api/products",
                async (
                    HttpRequest request,
                    IProductCatalogService catalog,
                    IOptions<HostedDemoOptions> hostedDemo,
                    CancellationToken cancellationToken) =>
                {
                    if (!IsSameOriginMutation(request))
                    {
                        return SameOriginRequired();
                    }

                    if (!request.HasFormContentType)
                    {
                        return Results.ValidationProblem(
                            Error("form", "Use a multipart garment submission."));
                    }

                    var form = await request.ReadFormAsync(cancellationToken);
                    var image = form.Files.GetFile("garmentImage");
                    var validation = ValidateProductForm(form, image);
                    if (validation.Count > 0)
                    {
                        return Results.ValidationProblem(validation);
                    }

                    ValidatedImage upload;
                    try
                    {
                        upload = await ReadImageAsync(
                            image!,
                            minimumWidth: 1_024,
                            minimumHeight: 1_024,
                            cancellationToken);
                    }
                    catch (InvalidDataException exception)
                    {
                        return Results.ValidationProblem(
                            Error("garmentImage", exception.Message));
                    }

                    if (hostedDemo.Value.SyntheticOnly &&
                        !MatchesSha256(
                            upload.Sha256,
                            hostedDemo.Value.AllowedGarmentImageSha256))
                    {
                        return SyntheticFixtureRequired();
                    }

                    try
                    {
                        var submission = new ProductCatalogSubmission(
                            Guid.Parse(form["boutiqueApplicationId"].ToString()),
                            form["name"].ToString(),
                            form["sku"].ToString(),
                            form["category"].ToString(),
                            form["brand"].ToString(),
                            form["color"].ToString(),
                            form["material"].ToString(),
                            form["sizeRange"].ToString(),
                            form["description"].ToString(),
                            ParsePrice(form["price"].ToString()),
                            form["currency"].ToString(),
                            NullIfWhiteSpace(form["productUrl"].ToString()),
                            upload.FileName,
                            upload.MediaType,
                            upload.PixelWidth,
                            upload.PixelHeight,
                            upload.Content);
                        return Results.Ok(
                            await catalog.SubmitAsync(
                                submission,
                                cancellationToken));
                    }
                    catch (InvalidOperationException exception)
                    {
                        return Results.ValidationProblem(
                            Error("product", exception.Message));
                    }
                })
            .DisableAntiforgery()
            .RequireRateLimiting(SecurityRateLimitPolicies.Upload)
            .WithName("SubmitProduct")
            .WithTags("Catalog")
            .RequireAuthorization(DemoAccessPolicies.Retailer);

        endpoints.MapGet(
                "/api/products/{productId:guid}/garment-image",
                async (
                    Guid productId,
                    HttpResponse response,
                    ClaimsPrincipal user,
                    IProductCatalogService catalog,
                    CancellationToken cancellationToken) =>
                {
                    var product = await catalog.GetProductAsync(
                        productId,
                        cancellationToken);
                    if (product is null)
                    {
                        return Results.NotFound();
                    }

                    if (product.Status != "Approved"
                        && user.Identity?.IsAuthenticated != true)
                    {
                        return Results.NotFound();
                    }

                    var image = await catalog.GetGarmentImageAsync(
                        productId,
                        cancellationToken);
                    if (image is null)
                    {
                        return Results.NotFound();
                    }

                    SetPrivateImageHeaders(response);
                    return Results.File(
                        image.Content,
                        image.MediaType,
                        fileDownloadName: null);
                })
            .WithName("GetGarmentImage")
            .WithTags("Catalog");

        endpoints.MapPost(
                "/api/try-on-jobs",
                async (
                    HttpRequest request,
                    ITryOnService tryOnService,
                    IOptions<HostedDemoOptions> hostedDemo,
                    CancellationToken cancellationToken) =>
                {
                    if (!IsSameOriginMutation(request))
                    {
                        return SameOriginRequired();
                    }

                    if (!request.HasFormContentType)
                    {
                        return Results.ValidationProblem(
                            Error("form", "Use a multipart try-on submission."));
                    }

                    var form = await request.ReadFormAsync(cancellationToken);
                    var image = form.Files.GetFile("personImage");
                    if (!Guid.TryParse(
                            form["productId"].ToString(),
                            out var productId))
                    {
                        return Results.ValidationProblem(
                            Error("productId", "Choose an approved garment."));
                    }

                    if (!bool.TryParse(
                            form["consentAccepted"].ToString(),
                            out var consentAccepted)
                        || !consentAccepted)
                    {
                        return Results.ValidationProblem(
                            Error(
                                "consentAccepted",
                                "Consent is required before starting a virtual try-on."));
                    }

                    if (image is null)
                    {
                        return Results.ValidationProblem(
                            Error(
                                "personImage",
                                "Choose an authorized person image."));
                    }

                    ValidatedImage upload;
                    try
                    {
                        upload = await ReadImageAsync(
                            image,
                            minimumWidth: 512,
                            minimumHeight: 384,
                            cancellationToken);
                    }
                    catch (InvalidDataException exception)
                    {
                        return Results.ValidationProblem(
                            Error("personImage", exception.Message));
                    }

                    if (hostedDemo.Value.SyntheticOnly &&
                        !MatchesSha256(
                            upload.Sha256,
                            hostedDemo.Value.AllowedPersonImageSha256))
                    {
                        return SyntheticFixtureRequired();
                    }

                    try
                    {
                        var job = await tryOnService.SubmitAsync(
                            new TryOnJobSubmission(
                                productId,
                                consentAccepted,
                                ConsentVersion: "2026-07-26",
                                upload.FileName,
                                upload.MediaType,
                                upload.PixelWidth,
                                upload.PixelHeight,
                                upload.Content),
                            cancellationToken);
                        return Results.Ok(job);
                    }
                    catch (InvalidOperationException exception)
                    {
                        return Results.ValidationProblem(
                            Error("tryOn", exception.Message));
                    }
                })
            .DisableAntiforgery()
            .RequireRateLimiting(SecurityRateLimitPolicies.Upload)
            .WithName("SubmitTryOnJob")
            .WithTags("Consumer Try-On");

        endpoints.MapGet(
                "/api/try-on-jobs/{jobId:guid}",
                async (
                    Guid jobId,
                    ITryOnService tryOnService,
                    CancellationToken cancellationToken) =>
                {
                    var job = await tryOnService.GetAsync(jobId, cancellationToken);
                    return job is null ? Results.NotFound() : Results.Ok(job);
                })
            .WithName("GetTryOnJob")
            .WithTags("Consumer Try-On");

        endpoints.MapGet(
                "/api/try-on-jobs/{jobId:guid}/result",
                async (
                    Guid jobId,
                    HttpResponse response,
                    ITryOnService tryOnService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await tryOnService.GetResultAsync(
                        jobId,
                        cancellationToken);
                    if (result is null)
                    {
                        return Results.NotFound();
                    }

                    SetPrivateImageHeaders(response);
                    return Results.File(
                        result.Content,
                        result.MediaType,
                        fileDownloadName: null);
                })
            .WithName("GetTryOnResult")
            .WithTags("Consumer Try-On");

        endpoints.MapGet(
                "/api/dashboard",
                async (
                    ITryOnService tryOnService,
                    CancellationToken cancellationToken) =>
                    TypedResults.Ok(
                        await tryOnService.GetDashboardAsync(cancellationToken)))
            .WithName("GetTryOnDashboard")
            .WithTags("Dashboard")
            .RequireAuthorization(
                DemoAccessPolicies.RetailerOrAdministrator);

        return endpoints;
    }

    private static Dictionary<string, string[]> ValidateProductForm(
        IFormCollection form,
        IFormFile? image)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        RequiredGuid(form, "boutiqueApplicationId", errors);
        Required(form, "name", 180, errors);
        Required(form, "sku", 80, errors);
        Required(form, "category", 32, errors);
        Required(form, "brand", 120, errors);
        Required(form, "color", 80, errors);
        Required(form, "material", 160, errors);
        Required(form, "sizeRange", 120, errors);
        Required(form, "description", 2000, errors);
        Required(form, "currency", 3, errors);

        if (image is null)
        {
            errors["garmentImage"] = ["Choose an authorized garment image."];
        }

        return errors;
    }

    private static async Task<ValidatedImage> ReadImageAsync(
        IFormFile image,
        int minimumWidth,
        int minimumHeight,
        CancellationToken cancellationToken)
    {
        if (image.Length <= 0 || image.Length > MaximumImageBytes)
        {
            throw new InvalidDataException(
                "The image must be larger than zero bytes and no more than 10 MB.");
        }

        if (!SupportedMediaTypes.Contains(image.ContentType))
        {
            throw new InvalidDataException(
                "Choose a JPEG, PNG, or WebP image.");
        }

        using var memory = new MemoryStream((int)image.Length);
        await image.CopyToAsync(memory, cancellationToken);
        var content = memory.ToArray();
        var information = Image.Identify(content);
        if (information is null)
        {
            throw new InvalidDataException(
                "The selected file is not a readable image.");
        }

        if (information.Width < minimumWidth
            || information.Height < minimumHeight)
        {
            throw new InvalidDataException(
                $"Use an image at least {minimumWidth} pixels wide and {minimumHeight} pixels tall.");
        }

        if (information.Width > 4096 || information.Height > 4096)
        {
            throw new InvalidDataException(
                "The image dimensions must not exceed 4096 pixels.");
        }

        return new ValidatedImage(
            Path.GetFileName(image.FileName),
            image.ContentType,
            information.Width,
            information.Height,
            Convert.ToHexString(SHA256.HashData(content)),
            content);
    }

    private static void Required(
        IFormCollection form,
        string field,
        int maximumLength,
        IDictionary<string, string[]> errors)
    {
        var value = form[field].ToString();
        if (string.IsNullOrWhiteSpace(value))
        {
            errors[field] = [$"{field} is required."];
        }
        else if (value.Length > maximumLength)
        {
            errors[field] = [$"{field} is too long."];
        }
    }

    private static void RequiredGuid(
        IFormCollection form,
        string field,
        IDictionary<string, string[]> errors)
    {
        if (!Guid.TryParse(form[field].ToString(), out var parsed)
            || parsed == Guid.Empty)
        {
            errors[field] = [$"{field} is required."];
        }
    }

    private static decimal? ParsePrice(string value) =>
        decimal.TryParse(
            value,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out var parsed)
            ? parsed
            : null;

    private static string? NullIfWhiteSpace(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static Dictionary<string, string[]> Error(
        string key,
        string message) =>
        new(StringComparer.Ordinal)
        {
            [key] = [message],
        };

    private static bool IsSameOriginMutation(HttpRequest request)
    {
        if (!request.Headers.TryGetValue(
                "X-TryOnReady-Request",
                out var verification) ||
            verification.Count != 1 ||
            verification[0] != "judge-demo")
        {
            return false;
        }

        if (request.Headers.TryGetValue("Sec-Fetch-Site", out var fetchSite) &&
            fetchSite.Count == 1 &&
            fetchSite[0] is not ("same-origin" or "same-site" or "none"))
        {
            return false;
        }

        if (!request.Headers.TryGetValue("Origin", out var origin) ||
            origin.Count != 1)
        {
            return true;
        }

        return Uri.TryCreate(origin[0], UriKind.Absolute, out var originUri) &&
            string.Equals(
                originUri.Authority,
                request.Host.Value,
                StringComparison.OrdinalIgnoreCase);
    }

    private static IResult SameOriginRequired() =>
        Results.Problem(
            statusCode: StatusCodes.Status403Forbidden,
            title: "Same-origin request required",
            detail: "This upload must originate from the TryOnReady judge site.");

    private static IResult SyntheticFixtureRequired() =>
        Results.Problem(
            statusCode: StatusCodes.Status403Forbidden,
            title: "Synthetic fixture required",
            detail: "This hosted judge demo accepts only the approved synthetic fixture.");

    private static bool IsSha256(string value) =>
        value.Length == 64 && value.All(char.IsAsciiHexDigit);

    private static bool MatchesSha256(string actual, string expected)
    {
        if (!IsSha256(expected))
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(
            Convert.FromHexString(actual),
            Convert.FromHexString(expected));
    }

    private static void SetPrivateImageHeaders(HttpResponse response)
    {
        response.Headers.CacheControl = "private, no-store, max-age=0";
        response.Headers.Pragma = "no-cache";
        response.Headers.XContentTypeOptions = "nosniff";
    }

    private sealed record ValidatedImage(
        string FileName,
        string MediaType,
        int PixelWidth,
        int PixelHeight,
        string Sha256,
        byte[] Content);
}

internal sealed class HostedDemoOptions
{
    public const string SectionName = "HostedDemo";

    public bool SyntheticOnly { get; set; }

    public string AllowedGarmentImageSha256 { get; set; } = string.Empty;

    public string AllowedPersonImageSha256 { get; set; } = string.Empty;
}
