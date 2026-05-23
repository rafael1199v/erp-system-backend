using Erp.Inventory.Contracts;
using Erp.Sales.Application.ContractServices;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Application.Services;
using Erp.Sales.Application.UseCases.Dashboard;
using Erp.Sales.Application.UseCases.KDS;
using Erp.Sales.Application.UseCases.RestaurantOrder;
using Erp.Sales.Application.UseCases.RestaurantOrderDetails;
using Erp.Sales.Application.UseCases.TaxConfiguration;
using Erp.Sales.Application.UseCases.Waiters;
using Erp.Sales.Infrastructure;
using Erp.Sales.Infrastructure.Http;
using Erp.Sales.Infrastructure.Pdf;
using Erp.Sales.Infrastructure.Repositories;
using Erp.Sales.Infrastructure.Services;
using Erp.Sales.Presentation.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;

namespace Erp.Sales.Presentation;

public static class SalesModule
{
    public static IServiceCollection AddSalesModule(this IServiceCollection services, IConfiguration configuration)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        services.AddMemoryCache();
        
        services.AddControllers().AddApplicationPart(typeof(TaxController).Assembly);
        services.AddControllers().AddApplicationPart(typeof(OrderController).Assembly);
        services.AddControllers().AddApplicationPart(typeof(OrderDetailController).Assembly);
        services.AddControllers().AddApplicationPart(typeof(WaiterController).Assembly);
        services.AddControllers().AddApplicationPart(typeof(KdsController).Assembly);
        services.AddControllers().AddApplicationPart(typeof(PaymentController).Assembly);
        services.AddControllers().AddApplicationPart(typeof(DashboardController).Assembly);
        
        services.AddDbContext<SalesDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(configuration["ConnectionStrings:DefaultConnection"]);
        });
        
        services.AddScoped<IUpdateGlobalTaxUseCase, UpdateGlobalTaxUseCase>();
        services.AddScoped<ICreateGlobalTaxUseCase, CreateGlobalTaxUseCase>();
        services.AddScoped<IGetGlobalTaxUseCase, GetGlobalTaxUseCase>();
        services.AddScoped<IGetTaxConfigurationUseCase, GetTaxConfigurationUseCase>();
        services.AddScoped<IUpsertGlobalTaxUseCase, UpsertGlobalTaxUseCase>();
        services.AddScoped<ICreateRestaurantOrderUseCase, CreateRestaurantOrderUseCase>();
        services.AddScoped<ICreateRestaurantOrderDetailUseCase, CreateRestaurantOrderDetailUseCase>();
        services.AddScoped<IUpdateRestaurantOrderDetailQuantityUseCase, UpdateRestaurantOrderDetailQuantityUseCase>();
        services.AddScoped<IGetOrderDetailProductsUseCase, GetOrderDetailProductsUseCase>();
        services.AddScoped<IGetRestaurantOrdersUseCase, GetRestaurantOrdersUseCase>();
        services.AddScoped<IGetWaitersByCompanyUseCase, GetWaitersByCompanyUseCase>();
        services.AddScoped<IGetWaiterOptionsByCompanyUseCase, GetWaiterOptionsByCompanyUseCase>();
        services.AddScoped<IAssignWaiterUseCase, AssignWaiterUseCase>();
        services.AddScoped<ICancelRestaurantOrderUseCase, CancelRestaurantOrderUseCase>();
        services.AddScoped<IGetRestaurantOrderDetailsUseCase, GetRestaurantOrderDetailsUseCase>();
        services.AddScoped<ISendOrderUseCase, SendOrderUseCase>();
        services.AddScoped<IGetRestaurantOrderTaxUseCase, GetRestaurantOrderTaxUseCase>();
        services.AddScoped<IGetTicketTotalsUseCase, GetTicketTotalsUseCase>();
        services.AddScoped<IGetKdsTeamsUseCase, GetKdsTeamsUseCase>();
        services.AddScoped<IGetKdsTeamItemsUseCase, GetKdsTeamItemsUseCase>();
        services.AddScoped<IChangeKdsItemStatusUseCase, ChangeKdsItemStatusUseCase>();
        services.AddScoped<IResendOrderDetailUseCase, ResendOrderDetailUseCase>();
        services.AddScoped<IPdfService, PdfService>();
        services.AddScoped<IPrintRestaurantOrderUseCase, PrintRestaurantOrderUseCase>();
        services.AddScoped<IPrintTicketContractUseCase, PrintTicketContractUseCase>();
        services.AddScoped<IGetPaymentTypesUseCase, GetPaymentTypesUseCase>();
        services.AddScoped<IProcessRestaurantOrderPaymentUseCase, ProcessRestaurantOrderPaymentUseCase>();
        services.AddScoped<IGetDailySalesDashboardUseCase, GetDailySalesDashboardUseCase>();
        services.AddScoped<IGetTopProductsDashboardUseCase, GetTopProductsDashboardUseCase>();
        services.AddScoped<IGetKdsStatusDashboardUseCase, GetKdsStatusDashboardUseCase>();
        services.AddScoped<ISalesDashboardContractService, SalesDashboardContractService>();
        services.AddScoped<ITicketContractService, TicketContractService>();
        
        services.AddScoped<ITaxConfigurationRepository, TaxConfigurationRepository>();
        services.AddScoped<IRestaurantOrderRepository, RestaurantOrderRepository>();
        services.AddScoped<IRestaurantOrderDetailRepository, RestaurantOrderDetailRepository>();
        services.AddScoped<IWaiterRepository, WaiterRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IWarehouseConfigurationRepository, WarehouseConfigurationRepository>();
        services.AddScoped<IKdsRepository, KdsRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<IPaymentTypeRepository, PaymentTypeRepository>();
        services.AddScoped<IPaymentProcessRepository, PaymentProcessRepository>();
        services.AddScoped<ISalesCenResolver, SalesCenResolver>();
        services.AddScoped<ISalesPaymentResolver, SalesPaymentResolver>();

        services.AddHttpClient<IInventoryService, InventoryHttpClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["Modules:InventoryBaseUrl"] ??
                                         throw new Exception("InventoryBaseUrl not found"));
        });
        
        return services;
    }
}
