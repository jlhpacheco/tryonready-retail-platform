using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace TryOnReady.Api.Authentication;

internal static class DemoAccessExtensions
{
    public static IServiceCollection AddTryOnReadyDemoAccess(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<DemoAccessOptions>(
            configuration.GetSection(DemoAccessOptions.SectionName));
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(
                options =>
                {
                    // A non-prefixed name allows the HTTP-only local judge
                    // demo to receive the cookie. Fly.io serves the same
                    // cookie over HTTPS, where SameAsRequest marks it Secure.
                    options.Cookie.Name = "TryOnReadyDemo";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                    options.SlidingExpiration = false;
                    options.LoginPath = "/sign-in/";
                    options.Events.OnRedirectToLogin = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.StatusCode =
                                StatusCodes.Status401Unauthorized;
                        }
                        else
                        {
                            context.Response.Redirect(context.RedirectUri);
                        }

                        return Task.CompletedTask;
                    };
                    options.Events.OnRedirectToAccessDenied = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.StatusCode =
                                StatusCodes.Status403Forbidden;
                        }
                        else
                        {
                            context.Response.Redirect("/sign-in/?denied=true");
                        }

                        return Task.CompletedTask;
                    };
                });
        services.AddAuthorization(
            options =>
            {
                options.AddPolicy(
                    DemoAccessPolicies.Retailer,
                    policy => policy.RequireRole("Retailer"));
                options.AddPolicy(
                    DemoAccessPolicies.Administrator,
                    policy => policy.RequireRole("Administrator"));
                options.AddPolicy(
                    DemoAccessPolicies.RetailerOrAdministrator,
                    policy => policy.RequireRole("Retailer", "Administrator"));
            });
        return services;
    }

    public static IEndpointRouteBuilder MapTryOnReadyDemoAccess(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
                "/api/auth/login",
                async (
                    DemoLoginRequest request,
                    HttpContext context,
                    IOptions<DemoAccessOptions> access) =>
                {
                    var account = ResolveAccount(request, access.Value);
                    if (account is null)
                    {
                        return Results.Json(
                            new
                            {
                                message =
                                    "The username or password was not recognized.",
                            },
                            statusCode: StatusCodes.Status401Unauthorized);
                    }

                    var (role, settings) = account.Value;
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.NameIdentifier, settings.Username),
                        new(ClaimTypes.Name, settings.DisplayName),
                        new(ClaimTypes.Role, role),
                    };
                    if (!string.IsNullOrWhiteSpace(settings.BoutiqueId))
                    {
                        claims.Add(new Claim("boutique_id", settings.BoutiqueId));
                    }

                    var identity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);
                    await context.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity),
                        new AuthenticationProperties
                        {
                            AllowRefresh = false,
                            IsPersistent = false,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                        });

                    return Results.Ok(
                        new
                        {
                            username = settings.Username,
                            displayName = settings.DisplayName,
                            role,
                            next =
                                role == "Administrator"
                                    ? "/admin-review/"
                                    : "/boutique-application/",
                        });
                })
            .AllowAnonymous()
            .WithName("DemoLogin")
            .WithTags("Demo Access");

        endpoints.MapPost(
                "/api/auth/logout",
                async (HttpContext context) =>
                {
                    await context.SignOutAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme);
                    return Results.NoContent();
                })
            .RequireAuthorization()
            .WithName("DemoLogout")
            .WithTags("Demo Access");

        endpoints.MapGet(
                "/api/auth/me",
                (ClaimsPrincipal user) =>
                    Results.Ok(
                        new
                        {
                            isAuthenticated =
                                user.Identity?.IsAuthenticated == true,
                            displayName = user.Identity?.Name,
                            role = user.FindFirstValue(ClaimTypes.Role),
                        }))
            .AllowAnonymous()
            .WithName("GetDemoIdentity")
            .WithTags("Demo Access");

        return endpoints;
    }

    private static (string Role, DemoAccountOptions Settings)? ResolveAccount(
        DemoLoginRequest request,
        DemoAccessOptions options)
    {
        if (Matches(request, options.Retailer))
        {
            return ("Retailer", options.Retailer);
        }

        if (Matches(request, options.Administrator))
        {
            return ("Administrator", options.Administrator);
        }

        return null;
    }

    private static bool Matches(
        DemoLoginRequest request,
        DemoAccountOptions account)
    {
        if (string.IsNullOrWhiteSpace(account.Username)
            || string.IsNullOrWhiteSpace(account.Password))
        {
            return false;
        }

        return FixedTimeEquals(request.Username, account.Username)
            && FixedTimeEquals(request.Password, account.Password);
    }

    private static bool FixedTimeEquals(string? supplied, string configured)
    {
        var suppliedHash = SHA256.HashData(
            Encoding.UTF8.GetBytes(supplied ?? ""));
        var configuredHash = SHA256.HashData(
            Encoding.UTF8.GetBytes(configured));
        return CryptographicOperations.FixedTimeEquals(
            suppliedHash,
            configuredHash);
    }
}
