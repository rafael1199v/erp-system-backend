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

    private const int CenMaxLength = 80;
    private const int SkuMaxLength = 100;
    private const int DescriptionMaxLength = 500;
    private const int AbbreviationMaxLength = 20;
    private const int StationCodeMaxLength = 50;
    private const int ExternalReferenceMaxLength = 120;
    
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

        modelBuilder.Entity<Company>(entity =>
        {
            entity.Property(company => company.Cen).HasMaxLength(CenMaxLength);
            entity.HasIndex(company => company.Cen).IsUnique();
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(category => category.Cen).HasMaxLength(CenMaxLength);
            entity.Property(category => category.Description).HasMaxLength(DescriptionMaxLength);
            entity.HasIndex(category => new { category.CompanyId, category.Cen }).IsUnique();
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.Property(unit => unit.Cen).HasMaxLength(CenMaxLength);
            entity.Property(unit => unit.Abbreviation).HasMaxLength(AbbreviationMaxLength);
            entity.HasIndex(unit => new { unit.CompanyId, unit.Cen }).IsUnique();
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.Property(warehouse => warehouse.Cen).HasMaxLength(CenMaxLength);
            entity.HasIndex(warehouse => new { warehouse.CompanyId, warehouse.Cen }).IsUnique();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(product => product.Cen).HasMaxLength(CenMaxLength);
            entity.Property(product => product.Sku).HasMaxLength(SkuMaxLength);
            entity.Property(product => product.Description).HasMaxLength(DescriptionMaxLength);
            entity.Property(product => product.StationCode).HasMaxLength(StationCodeMaxLength);
            entity.HasIndex(product => new { product.CompanyId, product.Cen }).IsUnique();
        });

        modelBuilder.Entity<InventoryMovement>(entity =>
        {
            entity.Property(movement => movement.Cen).HasMaxLength(CenMaxLength);
            entity.Property(movement => movement.ExternalReference).HasMaxLength(ExternalReferenceMaxLength);
            entity.HasIndex(movement => movement.Cen).IsUnique();
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.Property(transaction => transaction.Cen).HasMaxLength(CenMaxLength);
            entity.HasIndex(transaction => transaction.Cen).IsUnique();
        });

        modelBuilder.HasDefaultSchema(Schema.Inventory);
        base.OnModelCreating(modelBuilder);
    }
}
