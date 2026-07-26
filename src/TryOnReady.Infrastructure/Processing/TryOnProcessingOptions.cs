namespace TryOnReady.Infrastructure.Processing;

public sealed class TryOnProcessingOptions
{
    public const string SectionName = "TryOnProcessing";

    public int PollIntervalSeconds { get; init; } = 3;

    public int MaxAttempts { get; init; } = 80;

    public int ApiUnitsPerTask { get; init; } = 1;
}

