using Microsoft.Extensions.Options;
using TryOnReady.YouCam;

namespace TryOnReady.UnitTests;

public sealed class YouCamProviderModeResolverTests
{
    private static readonly DateTimeOffset StartsAt =
        DateTimeOffset.Parse("2026-08-18T04:00:00Z");
    private static readonly DateTimeOffset EndsAt =
        DateTimeOffset.Parse("2026-09-01T03:45:00Z");

    [Theory]
    [InlineData("2026-08-18T03:59:59Z", YouCamProviderMode.StoredReplay)]
    [InlineData("2026-08-18T04:00:00Z", YouCamProviderMode.YouCamLive)]
    [InlineData("2026-09-01T03:44:59Z", YouCamProviderMode.YouCamLive)]
    [InlineData("2026-09-01T03:45:00Z", YouCamProviderMode.StoredReplay)]
    public void AutomaticWindow_SelectsModeFromUtcClock(
        string now,
        YouCamProviderMode expected)
    {
        var resolver = CreateResolver(
            new YouCamOptions
            {
                Enabled = true,
                AutomaticLiveWindowEnabled = true,
                LiveWindowStartsAtUtc = StartsAt,
                LiveWindowEndsAtUtc = EndsAt,
            },
            DateTimeOffset.Parse(now));

        Assert.Equal(expected, resolver.Current);
    }

    [Fact]
    public void ManualLiveMode_RemainsLiveWithoutWindow()
    {
        var resolver = CreateResolver(
            new YouCamOptions { Enabled = true },
            StartsAt.AddYears(-1));

        Assert.Equal(YouCamProviderMode.YouCamLive, resolver.Current);
    }

    [Fact]
    public void SimulationMode_RemainsStoredReplayWithoutWindow()
    {
        var resolver = CreateResolver(
            new YouCamOptions { SimulationEnabled = true },
            StartsAt);

        Assert.Equal(YouCamProviderMode.StoredReplay, resolver.Current);
    }

    private static YouCamProviderModeResolver CreateResolver(
        YouCamOptions options,
        DateTimeOffset now) =>
        new(Options.Create(options), new FixedTimeProvider(now));

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
