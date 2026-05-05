using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Application.Mappers;
using Erp.Inventory.Application.Services;
using Erp.Inventory.Application.UseCases.Category;
using Erp.Inventory.Application.UseCases.Company;
using Erp.Inventory.Application.UseCases.Dashboard;
using Erp.Inventory.Application.UseCases.Movement;
using Erp.Inventory.Application.UseCases.Product;
using Erp.Inventory.Application.UseCases.ProductWarehouse;
using Erp.Inventory.Application.UseCases.Supplier;
using Erp.Inventory.Application.UseCases.Transaction;
using Erp.Inventory.Application.UseCases.Unit;
using Erp.Inventory.Application.UseCases.Warehouse;
using Erp.Inventory.Contracts;
using Erp.Inventory.Infrastructure.Persistance.Context;
using Erp.Inventory.Infrastructure.Persistance.Repositories;
using Erp.Inventory.Infrastructure.Persistance.Services;
using Erp.Inventory.Presentation.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Erp.Inventory.Presentation;

public static class InventoryModule
{
    public static IServiceCollection AddInventoryModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers().AddApplicationPart(typeof(CompanyController).Assembly);
        services.AddControllers().AddApplicationPart(typeof(MovementController).Assembly);
        services.AddControllers().AddApplicationPart(typeof(ProductController).Assembly);
        services.AddControllers().AddApplicationPart(typeof(TransactionController).Assembly);
        services.AddControllers().AddApplicationPart(typeof(WarehouseController).Assembly);
        services.AddControllers().AddApplicationPart(typeof(CategoryController).Assembly);
        services.AddControllers().AddApplicationPart(typeof(UnitController).Assembly);
        services.AddControllers().AddApplicationPart(typeof(SupplierController).Assembly);
        services.AddControllers().AddApplicationPart(typeof(DashboardController).Assembly);
        
        services.AddScoped<IGetCompaniesUseCase, GetCompaniesUseCase>();

        services.AddDbContext<AppDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(configuration["ConnectionStrings:DefaultConnection"]);
        });
        
        services.AddScoped<IGetProductStockUseCase, GetProductStockUseCase>();
        services.AddScoped<IGetProductCatalogUseCase, GetProductCatalogUseCase>();
        services.AddScoped<IGetProductWithWarehousesUseCase, GetProductWithWarehouseUseCase>();
        services.AddScoped<ICreateAdjustmentMovementUseCase, CreateAdjustmentMovementUseCase>();
        services.AddScoped<IUpdateStockUseCase, UpdateStockUseCase>();
        services.AddScoped<IGetTransactionDetailsUseCase, GetTransactionDetailsUseCase>();
        services.AddScoped<IGetMovementsUseCase, GetMovementsUseCase>();
        services.AddScoped<IGetWarehousesUseCase, GetWarehousesUseCase>();
        services.AddScoped<ICreateMovementUseCase, CreateMovementUseCase>();
        services.AddScoped<ICreateCategoryUseCase, CreateCategoryUseCase>();
        services.AddScoped<ICreateUnitUseCase, CreateUnitUseCase>();
        services.AddScoped<IGetCategoriesByCompanyUseCase, GetCategoriesByCompanyUseCase>();
        services.AddScoped<IGetUnitsByCompanyUseCase, GetUnitsByCompanyUseCase>();
        services.AddScoped<IUpdateUnitUseCase, UpdateUnitUseCase>();
        services.AddScoped<IUpdateCategoryUseCase, UpdateCategoryUseCase>();
        services.AddScoped<ICreateOwnProductUseCase, CreateOwnProductUseCase>();
        services.AddScoped<IUpdateOwnProductUseCase, UpdateOwnProductUseCase>();
        services.AddScoped<IDeactivateProductUseCase, DeactivateProductUseCase>();
        services.AddScoped<IGetSuppliersByCompanyUseCase, GetSuppliersByCompanyUseCase>();
        services.AddScoped<IGetProductWithCompanyUseCase, GetProductWithCompanyUseCase>();
        services.AddScoped<IActiveProductUseCase, ActiveProductUseCase>();
        services.AddScoped<IGetLowStockDashboardUseCase, GetLowStockDashboardUseCase>();
        
        services.AddScoped<IInventoryMovementMapper, InventoryMovementMapper>();
        services.AddScoped<ICompanyMapper, CompanyMapper>();
        services.AddScoped<IProductMapper, ProductMapper>();
        services.AddScoped<ITransactionMapper, TransactionMapper>();
        services.AddScoped<IWarehouseMapper, WarehouseMapper>();

        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IInventoryMovementRepository, InventoryMovementRepository>();
        services.AddScoped<IProductWarehouseRepository, ProductWarehouseRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUnitRepository, UnitRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();

        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IInventoryCenResolver, InventoryCenResolver>();
        
        return services;
    }
}
