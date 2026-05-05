using Erp.Inventory.Infrastructure.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace Erp.Inventory.Infrastructure.Persistance.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }
    
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ProductStatus> Statuses => Set<ProductStatus>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<ProductWarehouse> ProductWarehouses => Set<ProductWarehouse>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<TransactionType> TransactionTypes => Set<TransactionType>();
    public DbSet<MovementStatus> MovementStatuses => Set<MovementStatus>();
    public DbSet<MovementType> MovementTypes => Set<MovementType>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<CoreProduct> CoreProducts => Set<CoreProduct>();
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductWarehouse>()
            .HasKey(pw => new { pw.ProductId, pw.WarehouseId });
        
        modelBuilder.Entity<Category>().ToTable("categories");
        modelBuilder.Entity<ProductStatus>().ToTable("product_statuses");
        modelBuilder.Entity<Company>().ToTable("companies");
        modelBuilder.Entity<Product>().ToTable("products");
        modelBuilder.Entity<Warehouse>().ToTable("warehouses");
        modelBuilder.Entity<ProductWarehouse>().ToTable("products_warehouses");
        modelBuilder.Entity<Transaction>().ToTable("transactions");
        modelBuilder.Entity<Supplier>().ToTable("suppliers");
        modelBuilder.Entity<TransactionType>().ToTable("transaction_types");
        modelBuilder.Entity<MovementStatus>().ToTable("movement_statuses");
        modelBuilder.Entity<MovementType>().ToTable("movement_types");
        modelBuilder.Entity<InventoryMovement>().ToTable("inventory_movements");
        modelBuilder.Entity<Unit>().ToTable("units");
        modelBuilder.Entity<CoreProduct>().ToTable("core_products");

        modelBuilder.HasDefaultSchema(Schema.Inventory);
        base.OnModelCreating(modelBuilder);
    }
}