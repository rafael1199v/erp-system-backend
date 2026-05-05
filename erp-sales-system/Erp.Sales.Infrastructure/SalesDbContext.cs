using Erp.Sales.Infrastructure.Models;
using Erp.Sales.Infrastructure.Models.PoS;
using Microsoft.EntityFrameworkCore;

namespace Erp.Sales.Infrastructure;

public class SalesDbContext(DbContextOptions<SalesDbContext> options) : DbContext(options)
{
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
        modelBuilder.Entity<TeamConfigurationModel>(entity => {
            entity.HasKey(e => new { e.CompanyId, e.CategoryId, e.TeamId });
        });
        
        modelBuilder.Entity<TaxConfigurationModel>(entity => {
            entity.HasKey(t => t.CompanyId);
        });

        modelBuilder.Entity<WarehouseConfigurationModel>(entity =>
        {
            entity.HasKey(e => e.CompanyId);
        });
        
        modelBuilder.HasDefaultSchema(Schemas.Sales);
        base.OnModelCreating(modelBuilder);
    }
}