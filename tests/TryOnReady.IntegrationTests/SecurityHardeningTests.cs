using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace TryOnReady.IntegrationTests;

public sealed class SecurityHardeningTests
{
    [Fact]
    public async Task ProductionResponses_IncludeSecurityHeaders_AndHideOpenApi()
    {
        using var factory = CreateProductionFactory();
        using var client = CreateHttpsClient(factory);

        using var response = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        AssertHeader(response, "Strict-Transport-Security", "max-age=");
        AssertHeader(response, "Content-Security-Policy", "frame-ancestors 'none'");
        AssertHeader(response, "X-Frame-Options", "DENY");
        AssertHeader(response, "X-Content-Type-Options", "nosniff");
        AssertHeader(response, "Referrer-Policy", "no-referrer");
        AssertHeader(response, "Permissions-Policy", "camera=()");

        using var openApi = await client.GetAsync("/openapi/v1.json");
        Assert.Equal(HttpStatusCode.NotFound, openApi.StatusCode);
    }

    [Fact]
    public async Task ProductionCookie_UsesHostPrefixAndStrictFlags()
    {
        using var factory = CreateProductionFactory();
        using var client = CreateHttpsClient(factory);

        using var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                username = "retailer@tryonready.demo",
                password = "TestRetailer!2026",
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var cookie = Assert.Single(response.Headers.GetValues("Set-Cookie"));
        Assert.Contains("__Host-TryOnReadyDemo=", cookie, StringComparison.Ordinal);
        Assert.Contains("path=/", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("secure", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("httponly", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("samesite=strict", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("domain=", cookie, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task LoginAndUploadRateLimits_ReturnTooManyRequests()
    {
        using var factory = CreateProductionFactory();
        using var client = CreateHttpsClient(factory);

        for (var attempt = 0; attempt < 8; attempt++)
        {
            using var rejected = await client.PostAsJsonAsync(
                "/api/auth/login",
                new { username = "nobody", password = "incorrect" });
            Assert.Equal(HttpStatusCode.Unauthorized, rejected.StatusCode);
        }

        using (var limitedLogin = await client.PostAsJsonAsync(
                   "/api/auth/login",
                   new { username = "nobody", password = "incorrect" }))
        {
            Assert.Equal(
                HttpStatusCode.TooManyRequests,
                limitedLogin.StatusCode);
        }

        for (var attempt = 0; attempt < 12; attempt++)
        {
            using var form = new MultipartFormDataContent();
            form.Add(new StringContent("not-a-product-id"), "productId");
            form.Add(new StringContent("true"), "consentAccepted");
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "/api/try-on-jobs")
            {
                Content = form,
            };
            request.Headers.Add("X-TryOnReady-Request", "judge-demo");
            using var rejected = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.BadRequest, rejected.StatusCode);
        }

        using var limitedForm = new MultipartFormDataContent();
        limitedForm.Add(
            new StringContent("not-a-product-id"),
            "productId");
        limitedForm.Add(new StringContent("true"), "consentAccepted");
        using var limitedRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/try-on-jobs")
        {
            Content = limitedForm,
        };
        limitedRequest.Headers.Add("X-TryOnReady-Request", "judge-demo");
        using var limitedUpload = await client.SendAsync(limitedRequest);
        Assert.Equal(HttpStatusCode.TooManyRequests, limitedUpload.StatusCode);
    }

    [Fact]
    public async Task ProtectedAndCrossSiteRequests_AreRejected()
    {
        using var factory = CreateProductionFactory();
        using var client = CreateHttpsClient(factory);

        using (var unauthorized = await client.GetAsync("/api/dashboard"))
        {
            Assert.Equal(HttpStatusCode.Unauthorized, unauthorized.StatusCode);
        }

        using (var login = await client.PostAsJsonAsync(
                   "/api/auth/login",
                   new
                   {
                       username = "retailer@tryonready.demo",
                       password = "TestRetailer!2026",
                   }))
        {
            Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        }

        using var form = new MultipartFormDataContent();
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/products")
        {
            Content = form,
        };
        request.Headers.Add("X-TryOnReady-Request", "judge-demo");
        request.Headers.Add("Origin", "https://attacker.example");
        using var response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ProductionSyntheticOnlyMode_RejectsOtherImages()
    {
        using var factory = CreateProductionFactory(syntheticOnly: true);
        using var client = CreateHttpsClient(factory);

        using var image = new Image<Rgba32>(
            1_024,
            1_024,
            new Rgba32(20, 40, 60));
        using var stream = new MemoryStream();
        await image.SaveAsPngAsync(stream);

        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(Guid.NewGuid().ToString()), "productId");
        form.Add(new StringContent("true"), "consentAccepted");
        var imageContent = new ByteArrayContent(stream.ToArray());
        imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        form.Add(imageContent, "personImage", "not-approved.png");

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/try-on-jobs")
        {
            Content = form,
        };
        request.Headers.Add("X-TryOnReady-Request", "judge-demo");
        using var response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private static WebApplicationFactory<Program> CreateProductionFactory(
        bool syntheticOnly = false)
    {
        var storageRoot = Path.Combine(
            Path.GetTempPath(),
            "tryonready-security-tests",
            Guid.NewGuid().ToString("N"));
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(
                builder =>
                {
                    builder.UseEnvironment("Production");
                    builder.ConfigureAppConfiguration(
                        (_, configuration) =>
                        {
                            configuration.AddInMemoryCollection(
                                new Dictionary<string, string?>
                                {
                                    ["AllowedHosts"] = "localhost",
                                    ["Persistence:Provider"] = "InMemory",
                                    ["Persistence:InMemoryDatabaseName"] =
                                        $"security-{Guid.NewGuid():N}",
                                    ["PrivateStorage:RootPath"] = storageRoot,
                                    ["YouCam:Enabled"] = "false",
                                    ["YouCam:SimulationEnabled"] = "true",
                                    ["DemoAccess:Retailer:Username"] =
                                        "retailer@tryonready.demo",
                                    ["DemoAccess:Retailer:Password"] =
                                        "TestRetailer!2026",
                                    ["DemoAccess:Retailer:DisplayName"] =
                                        "Elena Rivera",
                                    ["DemoAccess:Administrator:Username"] =
                                        "admin@tryonready.demo",
                                    ["DemoAccess:Administrator:Password"] =
                                        "TestAdmin!2026",
                                    ["DemoAccess:Administrator:DisplayName"] =
                                        "Test Administrator",
                                    ["HostedDemo:SyntheticOnly"] =
                                        syntheticOnly.ToString(),
                                    ["HostedDemo:AllowedGarmentImageSha256"] =
                                        new string('0', 64),
                                    ["HostedDemo:AllowedPersonImageSha256"] =
                                        new string('0', 64),
                                });
                        });
                });
    }

    private static HttpClient CreateHttpsClient(
        WebApplicationFactory<Program> factory) =>
        factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("https://localhost"),
            });

    private static void AssertHeader(
        HttpResponseMessage response,
        string name,
        string expectedFragment)
    {
        Assert.True(response.Headers.TryGetValues(name, out var values));
        Assert.Contains(
            expectedFragment,
            string.Join(",", values),
            StringComparison.OrdinalIgnoreCase);
    }
}
