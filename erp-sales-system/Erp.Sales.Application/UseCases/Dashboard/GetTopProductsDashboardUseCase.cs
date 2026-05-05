using Erp.Inventory.Contracts;
using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;

namespace Erp.Sales.Application.UseCases.Dashboard;

public class GetTopProductsDashboardUseCase(
    IDashboardRepository dashboardRepository,
    IInventoryService inventoryService) : IGetTopProductsDashboardUseCase
{
    private const int DefaultTopN = 10;
    private const int MaxTopN = 100;

    public async Task<List<TopProductDashboardDto>> ExecuteAsync(int companyId, int topN)
    {
        int normalizedTopN = NormalizeTopN(topN);
        var (startUtc, endUtc) = BoliviaDateRangeHelper.GetCurrentDayUtcRange();

        var topProducts = await dashboardRepository.GetTopSoldProductsAsync(companyId, startUtc, endUtc, normalizedTopN);

        if (topProducts.Count == 0)
        {
            return [];
        }

        var productIds = topProducts.Select(p => p.ProductId).ToList();
        var productsById = (await inventoryService.GetOrderDetailProductsByIdsAsync(productIds))
            .ToDictionary(p => p.ProductId, p => p);

        return [.. topProducts.Select(tp =>
        {
            bool productExists = productsById.TryGetValue(tp.ProductId, out var product);

            return new TopProductDashboardDto(
                ProductId: tp.ProductId,
                ProductName: productExists ? product!.Name : $"Producto {tp.ProductId}",
                TotalQuantity: tp.TotalQuantity,
                CategoryId: productExists ? product!.CategoryId : 0,
                SellPrice: productExists ? product!.SellPrice : 0);
        })];
    }

    private static int NormalizeTopN(int topN)
    {
        if (topN <= 0)
        {
            return DefaultTopN;
        }

        return topN > MaxTopN ? MaxTopN : topN;
    }
}