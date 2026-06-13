using Erp.Purchasing.Application.Repositories;
using Erp.Purchasing.Application.Services;
using Erp.Purchasing.Application.UseCases;
using Erp.Purchasing.Infrastructure.Http;
using Erp.Purchasing.Infrastructure.Persistence;
using Erp.Purchasing.Infrastructure.Persistence.Context;
using Erp.Purchasing.Infrastructure.Persistence.Repositories;
using Erp.Purchasing.Presentation.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Erp.Purchasing.Presentation;

public static class PurchasingModule
{
    public static IServiceCollection AddPurchasingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers().AddApplicationPart(typeof(PurchaseOrderController).Assembly);
        services.AddControllers().AddApplicationPart(typeof(SupplierController).Assembly);

        services.AddDbContext<PurchasingDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(configuration["ConnectionStrings:DefaultConnection"]);
        });

        services.AddScoped<ICreatePurchaseUseCase, CreatePurchaseUseCase>();
        services.AddScoped<IConfirmPurchaseUseCase, ConfirmPurchaseUseCase>();
        services.AddScoped<IGetPurchaseUseCase, GetPurchaseUseCase>();
        services.AddScoped<IGetSuppliersUseCase, GetSuppliersUseCase>();

        services.AddScoped<IPurchaseRepository, PurchaseRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        var inventoryUrl = configuration["INVENTORY_URL"]
                           ?? configuration["Modules:InventoryBaseUrl"]
                           ?? throw new InvalidOperationException("INVENTORY_URL not configured");

        services.AddTransient<InventoryFailureTranslatingHandler>();

        services.AddHttpClient<IPurchasingInventoryService, PurchasingInventoryHttpClient>(client =>
            {
                client.BaseAddress = new Uri(inventoryUrl);
                client.Timeout = TimeSpan.FromSeconds(10);
            })
            .AddHttpMessageHandler<InventoryFailureTranslatingHandler>()
            .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))))
            .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

        return services;
    }
}
