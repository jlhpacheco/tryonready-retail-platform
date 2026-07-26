using TryOnReady.Application.BoutiqueApplications;

namespace TryOnReady.Infrastructure.Synthetic;

public sealed class InMemoryBoutiqueApplicationService
    : IBoutiqueApplicationService
{
    private readonly Lock syncRoot = new();
    private readonly List<BoutiqueApplicationView> applications = [];

    public Task<IReadOnlyList<BoutiqueApplicationView>> GetApplicationsAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (syncRoot)
        {
            return Task.FromResult<IReadOnlyList<BoutiqueApplicationView>>(
                applications.OrderByDescending(application => application.SubmittedAtUtc)
                    .ToArray());
        }
    }

    public Task<BoutiqueApplicationView> SubmitAsync(
        string boutiqueName,
        string ownerName,
        string email,
        int employeeCount,
        string primarySalesChannel,
        string? website,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var application = new BoutiqueApplicationView(
            Guid.NewGuid(),
            boutiqueName.Trim(),
            ownerName.Trim(),
            email.Trim(),
            employeeCount,
            primarySalesChannel.Trim(),
            string.IsNullOrWhiteSpace(website) ? null : website.Trim(),
            "Submitted",
            DateTimeOffset.UtcNow,
            null);

        lock (syncRoot)
        {
            applications.Add(application);
        }

        return Task.FromResult(application);
    }

    public Task<BoutiqueApplicationView?> RecordDecisionAsync(
        Guid applicationId,
        string decision,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (syncRoot)
        {
            var index = applications.FindIndex(
                application => application.Id == applicationId);

            if (index < 0)
            {
                return Task.FromResult<BoutiqueApplicationView?>(null);
            }

            var application = applications[index] with
            {
                Status = decision,
                DecidedAtUtc = DateTimeOffset.UtcNow,
            };
            applications[index] = application;
            return Task.FromResult<BoutiqueApplicationView?>(application);
        }
    }
}
