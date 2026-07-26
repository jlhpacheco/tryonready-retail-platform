namespace TryOnReady.Api.Contracts;

public sealed record ProductReadinessRequest(
    Guid ProductId,
    string? FileName,
    string? MediaType,
    long ByteLength,
    int PixelWidth,
    int PixelHeight);
