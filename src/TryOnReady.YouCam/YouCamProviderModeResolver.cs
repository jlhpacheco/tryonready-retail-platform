using Microsoft.Extensions.Options;

namespace TryOnReady.YouCam;

public sealed class YouCamProviderModeResolver(
    IOptions<YouCamOptions> options,
    TimeProvider timeProvider)
{
    private readonly YouCamOptions options = options.Value;

    public YouCamProviderMode Current
    {
        get
        {
            if (options.Enabled && IsInsideAutomaticWindow())
            {
                return YouCamProviderMode.YouCamLive;
            }

            if (options.SimulationEnabled || options.AutomaticLiveWindowEnabled)
            {
                return YouCamProviderMode.StoredReplay;
            }

            return YouCamProviderMode.Disabled;
        }
    }

    private bool IsInsideAutomaticWindow()
    {
        if (!options.AutomaticLiveWindowEnabled)
        {
            return true;
        }

        var now = timeProvider.GetUtcNow();
        return options.LiveWindowStartsAtUtc is { } startsAt
            && options.LiveWindowEndsAtUtc is { } endsAt
            && now >= startsAt
            && now < endsAt;
    }
}

public enum YouCamProviderMode
{
    Disabled,
    StoredReplay,
    YouCamLive,
}
