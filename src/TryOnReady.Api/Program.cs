using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TryOnReady.Api.Authentication;
using TryOnReady.Api.Endpoints;
using TryOnReady.Api.Health;
using TryOnReady.Api.Security;
using TryOnReady.Application.Readiness;
using TryOnReady.Infrastructure;
using TryOnReady.YouCam;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database")
    .AddCheck<PrivateStorageHealthCheck>("private-storage")
    .AddCheck<MemoryHealthCheck>("memory");
builder.Services.AddTryOnReadyDemoAccess(
    builder.Configuration,
    builder.Environment);
builder.Services.ConfigureWorkflowUploads(builder.Configuration);
builder.Services.AddTryOnReadySecurity();
builder.Services.AddSingleton<IProductReadinessService, ProductReadinessService>();
builder.Services.AddTryOnReadyInfrastructure(builder.Configuration);
builder.Services.AddYouCam(builder.Configuration);

var app = builder.Build();

if (args is ["migrate"])
{
    await app.Services.MigrateTryOnReadyDatabaseAsync();
    return;
}

app.UseTryOnReadySecurityHeaders();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

if (!app.Environment.IsProduction())
{
    app.MapOpenApi();
}
else
{
    app.Map("/openapi/{**path}", () => Results.NotFound());
}
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
