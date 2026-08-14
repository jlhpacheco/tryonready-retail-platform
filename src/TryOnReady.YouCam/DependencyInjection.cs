using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
                HasValidAutomaticLiveWindow,
                "YouCam automatic live mode requires real mode, a UTC start, and a later UTC end.")
            .Validate(
                options => options.RequestTimeoutSeconds is >= 10 and <= 300,
                "YouCam:RequestTimeoutSeconds must be between 10 and 300.")
            .ValidateOnStart();

        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<YouCamProviderModeResolver>();

        var providerOptions = configuration
            .GetSection(YouCamOptions.SectionName)
            .Get<YouCamOptions>() ?? new YouCamOptions();
        if (providerOptions.AutomaticLiveWindowEnabled)
        {
            services.AddSingleton<SimulatedYouCamGateway>();
            AddLiveGateway(services, providerOptions);
            services.AddSingleton<
                IApparelVirtualTryOnGateway,
                WindowedYouCamGateway>();
        }
        else if (providerOptions.SimulationEnabled)
        {
            services.AddSingleton<IApparelVirtualTryOnGateway, SimulatedYouCamGateway>();
        }
        else if (providerOptions.Enabled)
        {
            AddLiveGateway(services, providerOptions);
            services.AddTransient<IApparelVirtualTryOnGateway>(
                serviceProvider => serviceProvider.GetRequiredService<
                    YouCamApparelVirtualTryOnGateway>());
        }
        else
        {
            services.AddSingleton<
                IApparelVirtualTryOnGateway,
                DisabledYouCamGateway>();
        }

        return services;
    }

    private static void AddLiveGateway(
        IServiceCollection services,
        YouCamOptions providerOptions)
    {
        services.AddHttpClient<YouCamApparelVirtualTryOnGateway>(
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

    private static bool HasValidAutomaticLiveWindow(YouCamOptions options)
    {
        if (!options.AutomaticLiveWindowEnabled)
        {
            return true;
        }

        return options.Enabled
            && !options.SimulationEnabled
            && options.LiveWindowStartsAtUtc is { Offset: { Ticks: 0 } } startsAt
            && options.LiveWindowEndsAtUtc is { Offset: { Ticks: 0 } } endsAt
            && startsAt < endsAt;
    }
}
