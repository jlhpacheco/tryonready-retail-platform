using Microsoft.EntityFrameworkCore;
using TryOnReady.Application.Catalog;
using TryOnReady.Application.Readiness;
using TryOnReady.Application.Storage;
using TryOnReady.Domain.Products;

namespace TryOnReady.Infrastructure.Persistence;

internal sealed class PersistentProductCatalogService(
    IDbContextFactory<TryOnReadyDbContext> dbContextFactory,
    IPrivateAssetStore assetStore,
    IProductReadinessService readinessService)
    : IProductCatalogService
{
    public async Task<IReadOnlyList<ProductCatalogItem>> GetProductsAsync(
        string? status,
        CancellationToken cancellationToken)
    {
        await using var dbContext =
            await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var query = dbContext.Products
            .AsNoTracking()
            .Include(product => product.BoutiqueApplication)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(product => product.Status == status);
        }

        var products = await query
            .OrderByDescending(product => product.SubmittedAtUtc)
            .ToArrayAsync(cancellationToken);

        return products.Select(Map).ToArray();
    }

    public async Task<ProductCatalogItem?> GetProductAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        await using var dbContext =
            await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var product = await dbContext.Products
            .AsNoTracking()
            .Include(item => item.BoutiqueApplication)
            .SingleOrDefaultAsync(item => item.Id == productId, cancellationToken);
        return product is null ? null : Map(product);
    }

    public async Task<ProductImageContent?> GetGarmentImageAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        await using var dbContext =
            await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var product = await dbContext.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == productId, cancellationToken);
        if (product is null)
        {
            return null;
        }

        var asset = await assetStore.ReadAsync(
            product.GarmentAssetId,
            cancellationToken);
        return asset is null
            ? null
            : new ProductImageContent(
                asset.Metadata.FileName,
                asset.Metadata.MediaType,
                asset.Content);
    }

    public async Task<ProductCatalogItem> SubmitAsync(
        ProductCatalogSubmission submission,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(submission);

        var productId = Guid.NewGuid();
        var assessment = readinessService.Assess(
            new ProductImageMetadata(
                productId,
                submission.FileName,
                submission.MediaType,
                submission.Content.LongLength,
                submission.PixelWidth,
                submission.PixelHeight));

        if (!assessment.IsReady)
        {
            throw new InvalidOperationException(
                assessment.Issues.First().Message);
        }

        await using var dbContext =
            await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var application = await dbContext.BoutiqueApplications.SingleOrDefaultAsync(
            item => item.Id == submission.BoutiqueApplicationId,
            cancellationToken);

        if (application is null)
        {
            throw new InvalidOperationException(
                "Submit the boutique application before adding a garment.");
        }

        if (application.Status == "Declined")
        {
            throw new InvalidOperationException(
                "A declined boutique application cannot add garments.");
        }

        var stored = await assetStore.SaveAsync(
            new PrivateAssetUpload(
                submission.FileName,
                submission.MediaType,
                "garment",
                submission.Content,
                ExpiresAtUtc: null),
            cancellationToken);

        var entity = new ProductEntity
        {
            Id = productId,
            BoutiqueApplicationId = application.Id,
            BoutiqueApplication = application,
            Name = submission.Name.Trim(),
            Sku = submission.Sku.Trim(),
            Category = submission.Category.Trim(),
            Brand = submission.Brand.Trim(),
            Color = submission.Color.Trim(),
            Material = submission.Material.Trim(),
            SizeRange = submission.SizeRange.Trim(),
            Description = submission.Description.Trim(),
            Price = submission.Price,
            Currency = submission.Currency.Trim().ToUpperInvariant(),
            ProductUrl = string.IsNullOrWhiteSpace(submission.ProductUrl)
                ? null
                : submission.ProductUrl.Trim(),
            Status = "Pending",
            ReadinessPassed = true,
            GarmentAssetId = stored.Id,
            FileName = stored.FileName,
            MediaType = stored.MediaType,
            ByteLength = stored.ByteLength,
            PixelWidth = submission.PixelWidth,
            PixelHeight = submission.PixelHeight,
            SubmittedAtUtc = DateTimeOffset.UtcNow,
        };

        try
        {
            dbContext.Products.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
            return Map(entity);
        }
        catch
        {
            await assetStore.DeleteAsync(stored.Id, CancellationToken.None);
            throw;
        }
    }

    public async Task<ProductCatalogItem?> RecordDecisionAsync(
        Guid productId,
        string decision,
        string? notes,
        CancellationToken cancellationToken)
    {
        await using var dbContext =
            await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var product = await dbContext.Products
            .Include(item => item.BoutiqueApplication)
            .SingleOrDefaultAsync(item => item.Id == productId, cancellationToken);

        if (product is null)
        {
            return null;
        }

        if (decision == "Approved"
            && product.BoutiqueApplication.Status != "Approved")
        {
            throw new InvalidOperationException(
                "Approve the boutique application before approving its garment.");
        }

        product.Status = decision;
        product.DecisionNotes = string.IsNullOrWhiteSpace(notes)
            ? null
            : notes.Trim();
        product.DecidedAtUtc = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(product);
    }

    internal static ProductCatalogItem Map(ProductEntity product) =>
        new(
            product.Id,
            product.BoutiqueApplicationId,
            product.BoutiqueApplication.BoutiqueName,
            product.Name,
            product.Sku,
            product.Category,
            product.Brand,
            product.Color,
            product.Material,
            product.SizeRange,
            product.Description,
            product.Price,
            product.Currency,
            product.ProductUrl,
            product.Status,
            product.ReadinessPassed,
            $"/api/products/{product.Id}/garment-image",
            product.FileName,
            product.MediaType,
            product.ByteLength,
            product.PixelWidth,
            product.PixelHeight,
            product.SubmittedAtUtc,
            product.DecisionNotes,
            product.DecidedAtUtc);
}
