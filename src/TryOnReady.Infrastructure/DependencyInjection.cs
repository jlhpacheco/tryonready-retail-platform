using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TryOnReady.Application.AdminReview;
using TryOnReady.Application.BoutiqueApplications;
using TryOnReady.Application.Catalog;
using TryOnReady.Application.Storage;
using TryOnReady.Application.TryOn;
using TryOnReady.Infrastructure.Persistence;
using TryOnReady.Infrastructure.Processing;
using TryOnReady.Infrastructure.Storage;
using TryOnReady.Infrastructure.Synthetic;

namespace TryOnReady.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTryOnReadyInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<PersistenceOptions>()
            .Bind(configuration.GetSection(PersistenceOptions.SectionName))
            .Validate(
                options => options.Provider is "Postgres" or "InMemory",
                "Persistence:Provider must be Postgres or InMemory.")
            .ValidateOnStart();
        services.AddOptions<PrivateStorageOptions>()
            .Bind(configuration.GetSection(PrivateStorageOptions.SectionName))
            .Validate(
                options => options.PersonInputRetentionHours is >= 1 and <= 168,
                "PrivateStorage:PersonInputRetentionHours must be between 1 and 168.")
            .Validate(
                options => options.ResultRetentionHours is >= 1 and <= 720,
                "PrivateStorage:ResultRetentionHours must be between 1 and 720.")
            .ValidateOnStart();
        services.AddOptions<TryOnProcessingOptions>()
            .Bind(configuration.GetSection(TryOnProcessingOptions.SectionName))
            .Validate(
                options => options.PollIntervalSeconds is >= 1 and <= 30,
                "TryOnProcessing:PollIntervalSeconds must be between 1 and 30.")
            .Validate(
                options => options.MaxAttempts is >= 1 and <= 500,
                "TryOnProcessing:MaxAttempts must be between 1 and 500.")
            .Validate(
                options => options.ApiUnitsPerTask is >= 0 and <= 1000,
                "TryOnProcessing:ApiUnitsPerTask must be between 0 and 1000.")
            .ValidateOnStart();

        var persistence = configuration
            .GetSection(PersistenceOptions.SectionName)
            .Get<PersistenceOptions>() ?? new PersistenceOptions();
        services.AddDbContextFactory<TryOnReadyDbContext>(
            options =>
            {
                if (persistence.Provider == "InMemory")
                {
                    options.UseInMemoryDatabase(persistence.InMemoryDatabaseName);
                    return;
                }

                var connectionString = configuration.GetConnectionString("TryOnReady");
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException(
                        "ConnectionStrings:TryOnReady is required for PostgreSQL.");
                }

                options.UseNpgsql(
                    connectionString,
                    postgres => postgres.EnableRetryOnFailure(3));
            });

        services.AddSingleton<IDemoCatalog, SyntheticDemoCatalog>();
        services.AddSingleton<IPrivateAssetStore, FileSystemPrivateAssetStore>();
        services.AddSingleton<IAdminReviewService, PersistentAdminReviewService>();
        services.AddSingleton<IProductCatalogService, PersistentProductCatalogService>();
        services.AddSingleton<ITryOnService, PersistentTryOnService>();
        services.AddSingleton<
            IBoutiqueApplicationService,
            PersistentBoutiqueApplicationService>();
        services.AddHostedService<TryOnJobProcessor>();
        services.AddHostedService<PrivateAssetCleanupService>();
        return services;
    }

    public static async Task MigrateTryOnReadyDatabaseAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        await using var scope = serviceProvider.CreateAsyncScope();
        var factory = scope.ServiceProvider
            .GetRequiredService<IDbContextFactory<TryOnReadyDbContext>>();
        await using var dbContext =
            await factory.CreateDbContextAsync(cancellationToken);
        if (dbContext.Database.IsRelational())
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        else
        {
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        }
    }
}
