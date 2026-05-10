using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Erp.Purchasing.Presentation;

public static class PurchasingModule
{
    public static IServiceCollection AddPurchasingModule(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}