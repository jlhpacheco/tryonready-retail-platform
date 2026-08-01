using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace TryOnReady.IntegrationTests;

public sealed class CompleteJourneyTests : IClassFixture<TryOnReadyApiFactory>
{
    private readonly HttpClient client;

    public CompleteJourneyTests(TryOnReadyApiFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task BoutiqueToGeneratedResult_CompletesInSimulation()
    {
        await LoginAsync(
            "retailer@tryonready.demo",
            "PlaywrightRetailer!2026");

        using var applicationResponse = await client.PostAsJsonAsync(
            "/api/boutique-applications",
            new
            {
                boutiqueName = "Luna & Thread",
                ownerName = "Elena Rivera",
                email = "elena@luna-thread.example.invalid",
                employeeCount = 3,
                primarySalesChannel = "Physical store",
                website = (string?)null,
                certifiesImageRights = true,
            });
        Assert.Equal(HttpStatusCode.OK, applicationResponse.StatusCode);
        var applicationId = await ReadIdAsync(applicationResponse);

        var image = await CreateSyntheticImageAsync();
        using var productForm = new MultipartFormDataContent();
        Add(productForm, "boutiqueApplicationId", applicationId);
        Add(productForm, "name", "Moonlight Blazer");
        Add(productForm, "sku", "SYN-BLZ-001");
        Add(productForm, "category", "top");
        Add(productForm, "brand", "Luna & Thread");
        Add(productForm, "color", "Terracotta");
        Add(productForm, "material", "Cotton blend");
        Add(productForm, "sizeRange", "XS-XL");
        Add(
            productForm,
            "description",
            "A synthetic garment used for the verified integration journey.");
        Add(productForm, "price", "89.00");
        Add(productForm, "currency", "USD");
        productForm.Add(
            ImageContent(image),
            "garmentImage",
            "synthetic-garment.png");

        using var productResponse = await client.PostAsync(
            "/api/products",
            productForm);
        Assert.Equal(HttpStatusCode.OK, productResponse.StatusCode);
        var productId = await ReadIdAsync(productResponse);

        await LoginAsync(
            "admin@tryonready.demo",
            "PlaywrightAdmin!2026");

        using var boutiqueDecision = await client.PostAsJsonAsync(
            $"/api/boutique-applications/{applicationId}/decision",
            new { decision = "Approve" });
        Assert.Equal(HttpStatusCode.OK, boutiqueDecision.StatusCode);

        using var productDecision = await client.PostAsJsonAsync(
            $"/api/admin/reviews/{productId}/decision",
            new { decision = "Approve", notes = (string?)null });
        Assert.Equal(HttpStatusCode.OK, productDecision.StatusCode);

        using var logoutResponse = await client.PostAsync(
            "/api/auth/logout",
            content: null);
        Assert.Equal(HttpStatusCode.NoContent, logoutResponse.StatusCode);

        using var tryOnForm = new MultipartFormDataContent();
        Add(tryOnForm, "productId", productId);
        Add(tryOnForm, "consentAccepted", "true");
        tryOnForm.Add(
            ImageContent(image),
            "personImage",
            "synthetic-person.png");

        using var tryOnResponse = await client.PostAsync(
            "/api/try-on-jobs",
            tryOnForm);
        Assert.Equal(HttpStatusCode.OK, tryOnResponse.StatusCode);
        var jobId = await ReadIdAsync(tryOnResponse);

        JsonElement job = default;
        for (var attempt = 0; attempt < 40; attempt++)
        {
            using var jobResponse = await client.GetAsync(
                $"/api/try-on-jobs/{jobId}");
            Assert.Equal(HttpStatusCode.OK, jobResponse.StatusCode);
            job = await ReadJsonAsync(jobResponse);
            var status = job.GetProperty("status").GetString();
            if (status is "Succeeded" or "Failed")
            {
                break;
            }

            await Task.Delay(250);
        }

        Assert.Equal("Succeeded", job.GetProperty("status").GetString());
        Assert.False(job.GetProperty("isDuplicate").GetBoolean());
        Assert.Equal(0, job.GetProperty("apiUnitsConsumed").GetInt32());

        using var resultResponse = await client.GetAsync(
            $"/api/try-on-jobs/{jobId}/result");
        Assert.Equal(HttpStatusCode.OK, resultResponse.StatusCode);
        Assert.True(resultResponse.Headers.CacheControl?.Private);
        Assert.True(resultResponse.Headers.CacheControl?.NoStore);
        Assert.Equal(
            TimeSpan.Zero,
            resultResponse.Headers.CacheControl?.MaxAge);
        Assert.NotEmpty(await resultResponse.Content.ReadAsByteArrayAsync());

        await LoginAsync(
            "retailer@tryonready.demo",
            "PlaywrightRetailer!2026");

        using var dashboardResponse = await client.GetAsync("/api/dashboard");
        Assert.Equal(HttpStatusCode.OK, dashboardResponse.StatusCode);
        var dashboard = await ReadJsonAsync(dashboardResponse);
        Assert.Equal(1, dashboard.GetProperty("totalJobs").GetInt32());
        Assert.Equal(1, dashboard.GetProperty("succeededJobs").GetInt32());
        Assert.Equal(0, dashboard.GetProperty("apiUnitsConsumed").GetInt32());
    }

    [Fact]
    public async Task DuplicateBoutiqueApplication_ReturnsExistingApplication()
    {
        await LoginAsync(
            "retailer@tryonready.demo",
            "PlaywrightRetailer!2026");

        var email = $"elena+{Guid.NewGuid():N}@luna-thread.example.invalid";
        var request = new
        {
            boutiqueName = "Luna & Thread",
            ownerName = "Elena Rivera",
            email,
            employeeCount = 3,
            primarySalesChannel = "Physical store",
            website = (string?)null,
            certifiesImageRights = true,
        };

        using var firstResponse = await client.PostAsJsonAsync(
            "/api/boutique-applications",
            request);
        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        var firstId = await ReadIdAsync(firstResponse);

        using var duplicateResponse = await client.PostAsJsonAsync(
            "/api/boutique-applications",
            request with { email = email.ToUpperInvariant() });
        Assert.Equal(HttpStatusCode.OK, duplicateResponse.StatusCode);
        Assert.Equal(firstId, await ReadIdAsync(duplicateResponse));

        using var applicationsResponse = await client.GetAsync(
            "/api/boutique-applications");
        Assert.Equal(HttpStatusCode.OK, applicationsResponse.StatusCode);
        using var applications = JsonDocument.Parse(
            await applicationsResponse.Content.ReadAsStringAsync());
        Assert.Equal(
            1,
            applications.RootElement.EnumerateArray().Count(
                application => application.GetProperty("id").GetString() == firstId));
    }

    [Fact]
    public async Task DuplicateGarmentSku_ReturnsValidationProblemBeforeSecondReviewItem()
    {
        await LoginAsync(
            "retailer@tryonready.demo",
            "PlaywrightRetailer!2026");

        using var applicationResponse = await client.PostAsJsonAsync(
            "/api/boutique-applications",
            new
            {
                boutiqueName = "Luna & Thread",
                ownerName = "Elena Rivera",
                email = $"elena+{Guid.NewGuid():N}@luna-thread.example.invalid",
                employeeCount = 3,
                primarySalesChannel = "Physical store",
                website = (string?)null,
                certifiesImageRights = true,
            });
        Assert.Equal(HttpStatusCode.OK, applicationResponse.StatusCode);
        var applicationId = await ReadIdAsync(applicationResponse);

        var sku = $"SYN-BLZ-{Guid.NewGuid():N}"[..16].ToUpperInvariant();
        using var firstProductForm = await CreateProductFormAsync(applicationId, sku);
        using var firstProductResponse = await client.PostAsync(
            "/api/products",
            firstProductForm);
        Assert.Equal(HttpStatusCode.OK, firstProductResponse.StatusCode);

        using var duplicateProductForm = await CreateProductFormAsync(
            applicationId,
            sku.ToLowerInvariant());
        using var duplicateProductResponse = await client.PostAsync(
            "/api/products",
            duplicateProductForm);
        Assert.Equal(HttpStatusCode.BadRequest, duplicateProductResponse.StatusCode);
        var problem = await ReadJsonAsync(duplicateProductResponse);
        Assert.Contains(
            "already has a garment",
            problem.GetProperty("errors").GetProperty("product")[0].GetString());
    }

    private async Task LoginAsync(string username, string password)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new { username, password });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private static void Add(
        MultipartFormDataContent form,
        string name,
        string value) =>
        form.Add(new StringContent(value), name);

    private static ByteArrayContent ImageContent(byte[] image)
    {
        var content = new ByteArrayContent(image);
        content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        return content;
    }

    private static async Task<MultipartFormDataContent> CreateProductFormAsync(
        string applicationId,
        string sku)
    {
        var image = await CreateSyntheticImageAsync();
        var form = new MultipartFormDataContent();
        Add(form, "boutiqueApplicationId", applicationId);
        Add(form, "name", "Moonlight Blazer");
        Add(form, "sku", sku);
        Add(form, "category", "top");
        Add(form, "brand", "Luna & Thread");
        Add(form, "color", "Terracotta");
        Add(form, "material", "Cotton blend");
        Add(form, "sizeRange", "XS-XL");
        Add(
            form,
            "description",
            "A synthetic garment used for the verified integration journey.");
        Add(form, "price", "89.00");
        Add(form, "currency", "USD");
        form.Add(
            ImageContent(image),
            "garmentImage",
            "synthetic-garment.png");
        return form;
    }

    private static async Task<byte[]> CreateSyntheticImageAsync()
    {
        using var image = new Image<Rgba32>(
            1_024,
            1_024,
            new Rgba32(169, 87, 63));
        using var stream = new MemoryStream();
        await image.SaveAsPngAsync(stream);
        return stream.ToArray();
    }

    private static async Task<string> ReadIdAsync(HttpResponseMessage response)
    {
        var json = await ReadJsonAsync(response);
        return json.GetProperty("id").GetString()
            ?? throw new InvalidOperationException("The response did not contain an ID.");
    }

    private static async Task<JsonElement> ReadJsonAsync(
        HttpResponseMessage response)
    {
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        return document.RootElement.Clone();
    }
}
