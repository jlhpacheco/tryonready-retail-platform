namespace TryOnReady.Application.BoutiqueApplications;

public sealed record BoutiqueApplicationView(
    Guid Id,
    string BoutiqueName,
    string OwnerName,
    string Email,
    int EmployeeCount,
    string PrimarySalesChannel,
    string? Website,
    string Status,
    DateTimeOffset SubmittedAtUtc,
    DateTimeOffset? DecidedAtUtc);
