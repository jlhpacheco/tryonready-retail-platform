namespace TryOnReady.Application.BoutiqueApplications;

public interface IBoutiqueApplicationService
{
    Task<IReadOnlyList<BoutiqueApplicationView>> GetApplicationsAsync(
        CancellationToken cancellationToken);

    Task<BoutiqueApplicationView> SubmitAsync(
        string boutiqueName,
        string ownerName,
        string email,
        int employeeCount,
        string primarySalesChannel,
        string? website,
        CancellationToken cancellationToken);

    Task<BoutiqueApplicationView?> RecordDecisionAsync(
        Guid applicationId,
        string decision,
        CancellationToken cancellationToken);
}
