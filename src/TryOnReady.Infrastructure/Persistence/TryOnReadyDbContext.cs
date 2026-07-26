using Microsoft.EntityFrameworkCore;

namespace TryOnReady.Infrastructure.Persistence;

public sealed class TryOnReadyDbContext(
    DbContextOptions<TryOnReadyDbContext> options) : DbContext(options)
{
    public DbSet<BoutiqueApplicationEntity> BoutiqueApplications =>
        Set<BoutiqueApplicationEntity>();

    public DbSet<ProductEntity> Products => Set<ProductEntity>();

    public DbSet<TryOnJobEntity> TryOnJobs => Set<TryOnJobEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var applications = modelBuilder.Entity<BoutiqueApplicationEntity>();
        applications.ToTable("boutique_applications");
        applications.HasKey(application => application.Id);
        applications.Property(application => application.BoutiqueName).HasMaxLength(160);
        applications.Property(application => application.OwnerName).HasMaxLength(160);
        applications.Property(application => application.Email).HasMaxLength(320);
        applications.Property(application => application.PrimarySalesChannel)
            .HasMaxLength(80);
        applications.Property(application => application.Website).HasMaxLength(500);
        applications.Property(application => application.Status).HasMaxLength(32);
        applications.HasIndex(application => application.SubmittedAtUtc);

        var products = modelBuilder.Entity<ProductEntity>();
        products.ToTable("products");
        products.HasKey(product => product.Id);
        products.Property(product => product.Name).HasMaxLength(180);
        products.Property(product => product.Sku).HasMaxLength(80);
        products.Property(product => product.Category).HasMaxLength(32);
        products.Property(product => product.Brand).HasMaxLength(120);
        products.Property(product => product.Color).HasMaxLength(80);
        products.Property(product => product.Material).HasMaxLength(160);
        products.Property(product => product.SizeRange).HasMaxLength(120);
        products.Property(product => product.Description).HasMaxLength(2000);
        products.Property(product => product.Price).HasPrecision(12, 2);
        products.Property(product => product.Currency).HasMaxLength(3);
        products.Property(product => product.ProductUrl).HasMaxLength(500);
        products.Property(product => product.Status).HasMaxLength(32);
        products.Property(product => product.GarmentAssetId).HasMaxLength(64);
        products.Property(product => product.FileName).HasMaxLength(255);
        products.Property(product => product.MediaType).HasMaxLength(80);
        products.Property(product => product.DecisionNotes).HasMaxLength(2000);
        products.HasIndex(product => new { product.BoutiqueApplicationId, product.Sku })
            .IsUnique();
        products.HasIndex(product => product.Status);
        products.HasOne(product => product.BoutiqueApplication)
            .WithMany(application => application.Products)
            .HasForeignKey(product => product.BoutiqueApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        var jobs = modelBuilder.Entity<TryOnJobEntity>();
        jobs.ToTable("try_on_jobs");
        jobs.HasKey(job => job.Id);
        jobs.Property(job => job.Status).HasMaxLength(32);
        jobs.Property(job => job.Message).HasMaxLength(1000);
        jobs.Property(job => job.RequestFingerprint).HasMaxLength(64);
        jobs.Property(job => job.PersonAssetId).HasMaxLength(64);
        jobs.Property(job => job.PersonMediaType).HasMaxLength(80);
        jobs.Property(job => job.ConsentVersion).HasMaxLength(32);
        jobs.Property(job => job.ProviderReference).HasMaxLength(500);
        jobs.Property(job => job.ProviderErrorCode).HasMaxLength(160);
        jobs.Property(job => job.ResultAssetId).HasMaxLength(64);
        jobs.HasIndex(job => job.RequestFingerprint).IsUnique();
        jobs.HasIndex(job => new { job.Status, job.NextAttemptAtUtc });
        jobs.HasOne(job => job.Product)
            .WithMany(product => product.TryOnJobs)
            .HasForeignKey(job => job.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

