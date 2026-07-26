using Microsoft.EntityFrameworkCore;
using TryOnReady.Application.BoutiqueApplications;

namespace TryOnReady.Infrastructure.Persistence;

internal sealed class PersistentBoutiqueApplicationService(
    IDbContextFactory<TryOnReadyDbContext> dbContextFactory)
    : IBoutiqueApplicationService
{
    public async Task<IReadOnlyList<BoutiqueApplicationView>> GetApplicationsAsync(
        CancellationToken cancellationToken)
    {
        await using var dbContext =
            await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var applications = await dbContext.BoutiqueApplications
            .AsNoTracking()
            .OrderByDescending(application => application.SubmittedAtUtc)
            .ToArrayAsync(cancellationToken);

        return applications.Select(Map).ToArray();
    }

    public async Task<BoutiqueApplicationView> SubmitAsync(
        string boutiqueName,
        string ownerName,
        string email,
        int employeeCount,
        string primarySalesChannel,
        string? website,
        CancellationToken cancellationToken)
    {
        var entity = new BoutiqueApplicationEntity
        {
            Id = Guid.NewGuid(),
            BoutiqueName = boutiqueName.Trim(),
            OwnerName = ownerName.Trim(),
            Email = email.Trim(),
            EmployeeCount = employeeCount,
            PrimarySalesChannel = primarySalesChannel.Trim(),
            Website = string.IsNullOrWhiteSpace(website) ? null : website.Trim(),
            Status = "Submitted",
            SubmittedAtUtc = DateTimeOffset.UtcNow,
        };

        await using var dbContext =
            await dbContextFactory.CreateDbContextAsync(cancellationToken);
        dbContext.BoutiqueApplications.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<BoutiqueApplicationView?> RecordDecisionAsync(
        Guid applicationId,
        string decision,
        CancellationToken cancellationToken)
    {
        await using var dbContext =
            await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entity = await dbContext.BoutiqueApplications.FindAsync(
            [applicationId],
            cancellationToken);

        if (entity is null)
        {
            return null;
        }

        entity.Status = decision;
        entity.DecidedAtUtc = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    private static BoutiqueApplicationView Map(
        BoutiqueApplicationEntity application) =>
        new(
            application.Id,
            application.BoutiqueName,
            application.OwnerName,
            application.Email,
            application.EmployeeCount,
            application.PrimarySalesChannel,
            application.Website,
            application.Status,
            application.SubmittedAtUtc,
            application.DecidedAtUtc);
}
