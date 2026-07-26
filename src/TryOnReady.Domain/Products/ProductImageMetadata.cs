namespace TryOnReady.Domain.Products;

public sealed record ProductImageMetadata(
    Guid ProductId,
    string FileName,
    string MediaType,
    long ByteLength,
    int PixelWidth,
    int PixelHeight);
