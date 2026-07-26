using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace TryOnReady.IntegrationTests;

public sealed class TryOnReadyApiFactory : WebApplicationFactory<Program>
{
    private readonly string storageRoot = Path.Combine(
        Path.GetTempPath(),
        "tryonready-integration-tests",
        Guid.NewGuid().ToString("N"));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration(
            (_, configuration) =>
            {
                configuration.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["Persistence:Provider"] = "InMemory",
                        ["Persistence:InMemoryDatabaseName"] =
                            $"tryonready-integration-{Guid.NewGuid():N}",
                        ["PrivateStorage:RootPath"] = storageRoot,
                        ["PrivateStorage:PersonInputRetentionHours"] = "1",
                        ["PrivateStorage:ResultRetentionHours"] = "1",
                        ["TryOnProcessing:PollIntervalSeconds"] = "1",
                        ["TryOnProcessing:MaxAttempts"] = "20",
                        ["TryOnProcessing:ApiUnitsPerTask"] = "1",
                        ["YouCam:Enabled"] = "false",
                        ["YouCam:SimulationEnabled"] = "true",
                    });
            });
    }
}
