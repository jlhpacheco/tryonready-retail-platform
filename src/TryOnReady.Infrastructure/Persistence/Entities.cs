namespace TryOnReady.Infrastructure.Persistence;

public sealed class BoutiqueApplicationEntity
{
    public Guid Id { get; set; }

    public string BoutiqueName { get; set; } = string.Empty;

    public string OwnerName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public int EmployeeCount { get; set; }

    public string PrimarySalesChannel { get; set; } = string.Empty;

    public string? Website { get; set; }

    public string Status { get; set; } = "Submitted";

    public DateTimeOffset SubmittedAtUtc { get; set; }

    public DateTimeOffset? DecidedAtUtc { get; set; }

    public List<ProductEntity> Products { get; set; } = [];
}

public sealed class ProductEntity
{
    public Guid Id { get; set; }

    public Guid BoutiqueApplicationId { get; set; }

    public BoutiqueApplicationEntity BoutiqueApplication { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public string Sku { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Brand { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public string Material { get; set; } = string.Empty;

    public string SizeRange { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal? Price { get; set; }

    public string Currency { get; set; } = "USD";

    public string? ProductUrl { get; set; }

    public string Status { get; set; } = "Pending";

    public bool ReadinessPassed { get; set; }

    public string GarmentAssetId { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string MediaType { get; set; } = string.Empty;

    public long ByteLength { get; set; }

    public int PixelWidth { get; set; }

    public int PixelHeight { get; set; }

    public DateTimeOffset SubmittedAtUtc { get; set; }

    public string? DecisionNotes { get; set; }

    public DateTimeOffset? DecidedAtUtc { get; set; }

    public List<TryOnJobEntity> TryOnJobs { get; set; } = [];
}

public sealed class TryOnJobEntity
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public ProductEntity Product { get; set; } = null!;

    public string Status { get; set; } = "Pending";

    public string Message { get; set; } = string.Empty;

    public string RequestFingerprint { get; set; } = string.Empty;

    public int DuplicateRequestCount { get; set; }

    public string PersonAssetId { get; set; } = string.Empty;

    public string PersonMediaType { get; set; } = string.Empty;

    public string ConsentVersion { get; set; } = string.Empty;

    public DateTimeOffset ConsentAcceptedAtUtc { get; set; }

    public string? ProviderReference { get; set; }

    public string? ProviderErrorCode { get; set; }

    public string? ResultAssetId { get; set; }

    public int ApiUnitsReserved { get; set; }

    public int ApiUnitsConsumed { get; set; }

    public int AttemptCount { get; set; }

    public DateTimeOffset? NextAttemptAtUtc { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }

    public DateTimeOffset? CompletedAtUtc { get; set; }
}

