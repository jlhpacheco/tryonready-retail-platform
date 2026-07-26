namespace TryOnReady.Infrastructure.Storage;

public sealed class PrivateStorageOptions
{
    public const string SectionName = "PrivateStorage";

    public string RootPath { get; init; } = ".tryonready-data/private";

    public int PersonInputRetentionHours { get; init; } = 24;

    public int ResultRetentionHours { get; init; } = 24;
}

