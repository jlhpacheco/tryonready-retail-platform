namespace TryOnReady.Api.Contracts;

public sealed record BoutiqueApplicationRequest(
    string? BoutiqueName,
    string? OwnerName,
    string? Email,
    int EmployeeCount,
    string? PrimarySalesChannel,
    string? Website,
    bool CertifiesImageRights);
