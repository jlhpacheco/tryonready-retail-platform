using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TryOnReady.Api.Authentication;
using TryOnReady.Api.Endpoints;
using TryOnReady.Api.Health;
using TryOnReady.Application.Readiness;
using TryOnReady.Infrastructure;
using TryOnReady.YouCam;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database")
    .AddCheck<PrivateStorageHealthCheck>("private-storage")
    .AddCheck<MemoryHealthCheck>("memory");
builder.Services.AddTryOnReadyDemoAccess(builder.Configuration);
builder.Services.ConfigureWorkflowUploads();
builder.Services.AddSingleton<IProductReadinessService, ProductReadinessService>();
builder.Services.AddTryOnReadyInfrastructure(builder.Configuration);
builder.Services.AddYouCam(builder.Configuration);

var app = builder.Build();

if (args is ["migrate"])
{
    await app.Services.MigrateTryOnReadyDatabaseAsync();
    return;
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi();
app.MapHealthChecks(
    "/health",
    new HealthCheckOptions
    {
        AllowCachingResponses = false,
    });
app.MapScaffoldEndpoints();
app.MapWorkflowEndpoints();
app.MapTryOnReadyDemoAccess();
app.MapFallback(async context =>
{
    var requestedPath = context.Request.Path.Value?.Trim('/');
    var relativePagePath = string.IsNullOrWhiteSpace(requestedPath)
        ? "index.html"
        : $"{requestedPath}/index.html";
    var page = app.Environment.WebRootFileProvider.GetFileInfo(relativePagePath);

    if (!page.Exists)
    {
        page = app.Environment.WebRootFileProvider.GetFileInfo("index.html");
    }

    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync(page, context.RequestAborted);
});

app.Run();

public partial class Program;
