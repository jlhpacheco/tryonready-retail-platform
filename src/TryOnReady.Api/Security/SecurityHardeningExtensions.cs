using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace TryOnReady.Api.Security;

internal static class SecurityRateLimitPolicies
{
    public const string Login = "login";
    public const string Upload = "upload";
}

internal static class SecurityHardeningExtensions
{
    public static IServiceCollection AddTryOnReadySecurity(
        this IServiceCollection services)
    {
        services.AddHsts(
            options =>
            {
                options.MaxAge = TimeSpan.FromDays(180);
                options.IncludeSubDomains = false;
                options.Preload = false;
            });
        services.AddRateLimiter(
            options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddPolicy(
                    SecurityRateLimitPolicies.Login,
                    context => RateLimitPartition.GetFixedWindowLimiter(
                        GetClientKey(context),
                        _ => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 8,
                            QueueLimit = 0,
                            Window = TimeSpan.FromMinutes(1),
                        }));
                options.AddFixedWindowLimiter(
                    SecurityRateLimitPolicies.Upload,
                    limiter =>
                    {
                        limiter.AutoReplenishment = true;
                        limiter.PermitLimit = 12;
                        limiter.QueueLimit = 0;
                        limiter.Window = TimeSpan.FromMinutes(1);
                    });
            });
        services.AddSingleton<UploadConcurrencyGate>();
        return services;
    }

    public static IApplicationBuilder UseTryOnReadySecurityHeaders(
        this WebApplication app)
    {
        if (app.Environment.IsProduction())
        {
            app.UseHsts();
        }

        app.Use(
            async (context, next) =>
            {
                context.Response.OnStarting(
                    () =>
                    {
                        var headers = context.Response.Headers;
                        headers["Content-Security-Policy"] =
                            "default-src 'self'; base-uri 'self'; object-src 'none'; " +
                            "frame-ancestors 'none'; form-action 'self'; " +
                            "img-src 'self' data: blob:; " +
                            "script-src 'self' 'unsafe-inline'; " +
                            "style-src 'self' 'unsafe-inline'; connect-src 'self'";
                        headers["X-Frame-Options"] = "DENY";
                        headers["X-Content-Type-Options"] = "nosniff";
                        headers["Referrer-Policy"] = "no-referrer";
                        headers["Permissions-Policy"] =
                            "camera=(), microphone=(), geolocation=(), payment=()";
                        return Task.CompletedTask;
                    });
                await next();
            });
        app.Use(
            async (context, next) =>
            {
                if (!HttpMethods.IsPost(context.Request.Method) ||
                    (context.Request.Path != "/api/products" &&
                     context.Request.Path != "/api/try-on-jobs"))
                {
                    await next();
                    return;
                }

                var gate = context.RequestServices
                    .GetRequiredService<UploadConcurrencyGate>();
                if (!await gate.TryEnterAsync(context.RequestAborted))
                {
                    context.Response.StatusCode =
                        StatusCodes.Status429TooManyRequests;
                    context.Response.Headers.RetryAfter = "1";
                    return;
                }

                try
                {
                    await next();
                }
                finally
                {
                    gate.Exit();
                }
            });
        return app;
    }

    private static string GetClientKey(HttpContext context) =>
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}

internal sealed class UploadConcurrencyGate
{
    private readonly SemaphoreSlim gate = new(1, 1);

    public Task<bool> TryEnterAsync(CancellationToken cancellationToken) =>
        gate.WaitAsync(TimeSpan.Zero, cancellationToken);

    public void Exit() => gate.Release();
}
