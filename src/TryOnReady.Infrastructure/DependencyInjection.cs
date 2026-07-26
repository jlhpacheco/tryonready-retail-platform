using Microsoft.Extensions.DependencyInjection;
using TryOnReady.Application.AdminReview;
using TryOnReady.Application.BoutiqueApplications;
using TryOnReady.Application.Catalog;
using TryOnReady.Infrastructure.Synthetic;

namespace TryOnReady.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTryOnReadyInfrastructure(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IDemoCatalog, SyntheticDemoCatalog>();
        services.AddSingleton<IAdminReviewService, InMemoryAdminReviewService>();
        services.AddSingleton<
            IBoutiqueApplicationService,
            InMemoryBoutiqueApplicationService>();
        return services;
    }
}
