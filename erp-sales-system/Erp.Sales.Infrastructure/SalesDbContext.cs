using Erp.Sales.Infrastructure.Models;
using Erp.Sales.Infrastructure.Models.PoS;
using Microsoft.EntityFrameworkCore;

namespace Erp.Sales.Infrastructure;

public class SalesDbContext(DbContextOptions<SalesDbContext> options) : DbContext(options)
{
    private const int CenMaxLength = 80;

    // Core Tables
    public DbSet<CustomerModel> Customers => Set<CustomerModel>();
    public DbSet<OrderDetailModel> OrderDetails => Set<OrderDetailModel>();
    public DbSet<OrderModel> Orders => Set<OrderModel>();
    public DbSet<OrderStatusModel> OrderStatuses => Set<OrderStatusModel>();
    public DbSet<PaymentTypeModel> PaymentTypes => Set<PaymentTypeModel>();
    public DbSet<SaleDetailModel> SaleDetails => Set<SaleDetailModel>();
    public DbSet<SaleModel> Sales => Set<SaleModel>();
    public DbSet<TaxConfigurationModel> TaxConfigurations => Set<TaxConfigurationModel>();
    public DbSet<WarehouseConfigurationModel> WarehouseConfigurations => Set<WarehouseConfigurationModel>();
    
    // PoS Tables
    public DbSet<RestaurantOrderDetailModel> RestaurantOrderDetails => Set<RestaurantOrderDetailModel>();
    public DbSet<RestaurantOrderDetailStatusModel> RestaurantOrderDetailStatuses => Set<RestaurantOrderDetailStatusModel>();
    public DbSet<RestaurantOrderModel> RestaurantOrders => Set<RestaurantOrderModel>();
    public DbSet<TeamConfigurationModel> TeamConfigurations => Set<TeamConfigurationModel>();
    public DbSet<TeamModel> Teams => Set<TeamModel>();
    public DbSet<WaiterModel> Waiters => Set<WaiterModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerModel>(entity =>
        {
            entity.Property(customer => customer.CompanyCen).HasMaxLength(CenMaxLength);
            entity.HasIndex(customer => customer.CompanyCen);
        });

        modelBuilder.Entity<OrderDetailModel>(entity =>
        {
            entity.Property(detail => detail.ProductCen).HasMaxLength(CenMaxLength);
            entity.HasIndex(detail => detail.ProductCen);
        });

        modelBuilder.Entity<OrderModel>(entity =>
        {
            entity.Property(order => order.CompanyCen).HasMaxLength(CenMaxLength);
            entity.HasIndex(order => order.CompanyCen);
        });

        modelBuilder.Entity<SaleDetailModel>(entity =>
        {
            entity.Property(detail => detail.ProductCen).HasMaxLength(CenMaxLength);
            entity.HasIndex(detail => detail.ProductCen);
        });

        modelBuilder.Entity<SaleModel>(entity =>
        {
            entity.Property(sale => sale.Cen).HasMaxLength(CenMaxLength);
            entity.Property(sale => sale.CompanyCen).HasMaxLength(CenMaxLength);
            entity.HasIndex(sale => sale.Cen).IsUnique();
            entity.HasIndex(sale => sale.CompanyCen);
        });

        modelBuilder.Entity<RestaurantOrderModel>(entity =>
        {
            entity.Property(ticket => ticket.Cen).HasMaxLength(CenMaxLength);
            entity.HasIndex(ticket => ticket.Cen).IsUnique();
        });

        modelBuilder.Entity<RestaurantOrderDetailModel>(entity =>
        {
            entity.Property(ticketItem => ticketItem.Cen).HasMaxLength(CenMaxLength);
            entity.Property(ticketItem => ticketItem.ProductCen).HasMaxLength(CenMaxLength);
            entity.HasIndex(ticketItem => ticketItem.Cen).IsUnique();
            entity.HasIndex(ticketItem => ticketItem.ProductCen);
        });

        modelBuilder.Entity<TeamModel>(entity =>
        {
            entity.Property(team => team.Cen).HasMaxLength(CenMaxLength);
            entity.Property(team => team.CompanyCen).HasMaxLength(CenMaxLength);
            entity.HasIndex(team => new { team.CompanyId, team.Cen }).IsUnique();
            entity.HasIndex(team => team.CompanyCen);
        });

        modelBuilder.Entity<WaiterModel>(entity =>
        {
            entity.Property(waiter => waiter.Cen).HasMaxLength(CenMaxLength);
            entity.Property(waiter => waiter.CompanyCen).HasMaxLength(CenMaxLength);
            entity.HasIndex(waiter => new { waiter.CompanyId, waiter.Cen }).IsUnique();
            entity.HasIndex(waiter => waiter.CompanyCen);
        });

        modelBuilder.Entity<TeamConfigurationModel>(entity => {
            entity.HasKey(e => new { e.CompanyId, e.CategoryId, e.TeamId });
            entity.Property(configuration => configuration.CompanyCen).HasMaxLength(CenMaxLength);
            entity.Property(configuration => configuration.CategoryCen).HasMaxLength(CenMaxLength);
            entity.HasIndex(configuration => configuration.CompanyCen);
            entity.HasIndex(configuration => configuration.CategoryCen);
        });
        
        modelBuilder.Entity<TaxConfigurationModel>(entity => {
            entity.HasKey(t => t.CompanyId);
            entity.Property(tax => tax.CompanyCen).HasMaxLength(CenMaxLength);
            entity.HasIndex(tax => tax.CompanyCen);
        });

        modelBuilder.Entity<WarehouseConfigurationModel>(entity =>
        {
            entity.HasKey(e => e.CompanyId);
            entity.Property(configuration => configuration.CompanyCen).HasMaxLength(CenMaxLength);
            entity.Property(configuration => configuration.MainWarehouseCen).HasMaxLength(CenMaxLength);
            entity.HasIndex(configuration => configuration.CompanyCen);
            entity.HasIndex(configuration => configuration.MainWarehouseCen);
        });
        
        modelBuilder.HasDefaultSchema(Schemas.Sales);
        base.OnModelCreating(modelBuilder);
    }
}
