using Microsoft.Extensions.DependencyInjection;
using TryOnReady.Application.Catalog;
using TryOnReady.Infrastructure.Synthetic;

namespace TryOnReady.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTryOnReadyInfrastructure(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IDemoCatalog, SyntheticDemoCatalog>();
        return services;
    }
}
