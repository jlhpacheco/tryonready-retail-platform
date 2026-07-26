namespace TryOnReady.Domain.Boutiques;

public sealed record BoutiqueApplication(
    Guid Id,
    Guid BoutiqueId,
    BoutiqueApplicationStatus Status,
    DateTimeOffset? SubmittedAtUtc);

public enum BoutiqueApplicationStatus
{
    Draft,
    Submitted,
    UnderReview,
    Approved,
    Declined,
}
