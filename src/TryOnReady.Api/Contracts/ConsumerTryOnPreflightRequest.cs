namespace TryOnReady.Api.Contracts;

public sealed record ConsumerTryOnPreflightRequest(
    Guid ProductId,
    string? FileName,
    string? MediaType,
    long ByteLength,
    int PixelWidth,
    int PixelHeight,
    bool ConsentAccepted);
