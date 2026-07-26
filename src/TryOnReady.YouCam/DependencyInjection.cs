using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TryOnReady.Application.VirtualTryOn;

namespace TryOnReady.YouCam;

public static class DependencyInjection
{
    public static IServiceCollection AddYouCamScaffold(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddOptions<YouCamOptions>()
            .Bind(configuration.GetSection(YouCamOptions.SectionName))
            .Validate(
                options => options.BaseUrl.Scheme is "https",
                "YouCam:BaseUrl must use HTTPS.")
            .Validate(
                options => !options.Enabled || !string.IsNullOrWhiteSpace(options.ApiKey),
                "YouCam:ApiKey is required only when YouCam:Enabled is true.")
            .ValidateOnStart();

        services.AddSingleton<IApparelVirtualTryOnGateway, DisabledYouCamGateway>();
        return services;
    }
}
