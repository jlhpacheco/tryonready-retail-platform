using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TryOnReady.Application.VirtualTryOn;

namespace TryOnReady.YouCam;

public static class DependencyInjection
{
    public static IServiceCollection AddYouCam(
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
            .Validate(
                options => !(options.Enabled && options.SimulationEnabled),
                "YouCam real and simulation modes cannot both be enabled.")
            .Validate(
                options => options.RequestTimeoutSeconds is >= 10 and <= 300,
                "YouCam:RequestTimeoutSeconds must be between 10 and 300.")
            .ValidateOnStart();

        var providerOptions = configuration
            .GetSection(YouCamOptions.SectionName)
            .Get<YouCamOptions>() ?? new YouCamOptions();
        if (providerOptions.SimulationEnabled)
        {
            services.AddSingleton<IApparelVirtualTryOnGateway, SimulatedYouCamGateway>();
        }
        else if (providerOptions.Enabled)
        {
            services.AddHttpClient<
                    IApparelVirtualTryOnGateway,
                    YouCamApparelVirtualTryOnGateway>(
                    client =>
                    {
                        client.BaseAddress = providerOptions.BaseUrl;
                        client.Timeout = TimeSpan.FromSeconds(
                            providerOptions.RequestTimeoutSeconds);
                    })
                .ConfigurePrimaryHttpMessageHandler(
                    () => new SocketsHttpHandler
                    {
                        AllowAutoRedirect = false,
                        PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                    });
        }
        else
        {
            services.AddSingleton<
                IApparelVirtualTryOnGateway,
                DisabledYouCamGateway>();
        }

        return services;
    }
}
