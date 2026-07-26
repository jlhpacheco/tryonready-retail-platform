using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TryOnReady.IntegrationTests;

public sealed class ApiScaffoldTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    public ApiScaffoldTests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        using var response = await client.GetAsync("/health", CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task LandingPage_IsServedByApiHost()
    {
        using var response = await client.GetAsync("/", CancellationToken.None);
        var body = await response.Content.ReadAsStringAsync(CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Virtual try-on", body, StringComparison.Ordinal);
        Assert.Contains("TryOnReady", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ProductReadinessPage_IsServedWhenOpenedDirectly()
    {
        using var response = await client.GetAsync(
            "/product-readiness/",
            CancellationToken.None);
        var body = await response.Content.ReadAsStringAsync(CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Check a garment image", body, StringComparison.Ordinal);
        Assert.Contains("Garment name", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ApiEntryPoint_ListsAvailableScaffoldEndpoints()
    {
        using var response = await client.GetAsync("/api", CancellationToken.None);
        var body = await response.Content.ReadAsStringAsync(CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"liveYouCamIntegration\":false", body, StringComparison.Ordinal);
        Assert.Contains("/openapi/v1.json", body, StringComparison.Ordinal);
        Assert.Contains("/api/readiness/assess", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task OpenApiDocument_IsAvailable()
    {
        using var response = await client.GetAsync("/openapi/v1.json", CancellationToken.None);
        var body = await response.Content.ReadAsStringAsync(CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"openapi\"", body, StringComparison.Ordinal);
        Assert.Contains("/api/readiness/assess", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DemoCatalog_ReturnsSyntheticPersonaOnly()
    {
        using var response = await client.GetAsync(
            "/api/demo/catalog",
            CancellationToken.None);
        var body = await response.Content.ReadAsStringAsync(CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Luna & Thread", body, StringComparison.Ordinal);
        Assert.Contains("Elena Rivera", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Readiness_RejectsMissingBoundaryValues()
    {
        using var response = await client.PostAsJsonAsync(
            "/api/readiness/assess",
            new
            {
                productId = Guid.Empty,
                fileName = "",
                mediaType = "",
                byteLength = 0,
                pixelWidth = 0,
                pixelHeight = 0,
            },
            CancellationToken.None);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
