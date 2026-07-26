namespace TryOnReady.Domain.Boutiques;

public sealed record Boutique(
    Guid Id,
    string DisplayName,
    string OwnerDisplayName,
    int EmployeeCount);
