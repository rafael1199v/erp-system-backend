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
        modelBuilder.Entity<PurchaseModel>()
            .Property(p => p.PurchaseDate)
            .HasConversion<string>();
        
        modelBuilder.HasDefaultSchema(Schema.Purchasing);
        base.OnModelCreating(modelBuilder);
    }
}