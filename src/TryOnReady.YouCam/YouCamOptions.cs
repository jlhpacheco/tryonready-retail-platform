namespace TryOnReady.YouCam;

public sealed class YouCamOptions
{
    public const string SectionName = "YouCam";

    public bool Enabled { get; init; }

    public Uri BaseUrl { get; init; } = new("https://yce-api-01.makeupar.com");

    public string ApiKey { get; init; } = string.Empty;
}
