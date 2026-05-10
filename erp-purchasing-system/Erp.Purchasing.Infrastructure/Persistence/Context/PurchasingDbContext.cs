using Erp.Purchasing.Infrastructure.Persistence.Models;
using Erp.Purchasing.Infrastructure.Persistence.Schemas;
using Microsoft.EntityFrameworkCore;

namespace Erp.Purchasing.Infrastructure.Persistence.Context;

public class PurchasingDbContext(DbContextOptions<PurchasingDbContext> options) : DbContext(options)
{
    public DbSet<SupplierModel> Suppliers => Set<SupplierModel>();
    public DbSet<PurchaseModel> Purchases => Set<PurchaseModel>();
    public DbSet<PurchaseItemModel> PurchaseItems => Set<PurchaseItemModel>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PurchaseModel>(entity =>
        {
            entity.HasKey(purchase => purchase.Id);

            entity.Property(purchase => purchase.PurchaseStatus)
                .HasConversion<string>();

            entity.HasOne(purchase => purchase.Supplier)
                .WithMany()
                .HasForeignKey(purchase => purchase.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(purchase => purchase.PurchaseItems)
                .WithOne(item => item.Purchase)
                .HasForeignKey(item => item.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PurchaseItemModel>(entity =>
        {
            entity.HasKey(item => item.Id);
        });

        modelBuilder.Entity<SupplierModel>(entity =>
        {
            entity.HasKey(supplier => supplier.Id);
        });
        
        modelBuilder.HasDefaultSchema(Schema.Purchasing);
        base.OnModelCreating(modelBuilder);
    }
}
