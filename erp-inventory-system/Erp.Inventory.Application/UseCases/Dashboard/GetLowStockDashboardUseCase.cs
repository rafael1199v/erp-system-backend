using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.UseCases.Product;

namespace Erp.Inventory.Application.UseCases.Dashboard;

public class GetLowStockDashboardUseCase(IGetProductCatalogUseCase getProductCatalogUseCase) : IGetLowStockDashboardUseCase
{
    public async Task<List<LowStockDashboardDto>> ExecuteAsync(int companyId)
    {
        List<GetProductCatalogDTO> products = await getProductCatalogUseCase.ExecuteAsync(companyId);

        return [.. products
            .Where(p => p.IsActive && (p.TotalStock == 0 || p.TotalStock <= p.ReorderLevel))
            .Select(p => new LowStockDashboardDto(
                ProductId: p.ProductId,
                ProductName: p.ProductName,
                TotalStock: p.TotalStock,
                ReorderLevel: p.ReorderLevel,
                StockState: p.TotalStock == 0 ? "OutOfStock" : "LowStock"))
            .OrderBy(p => p.StockState)
            .ThenBy(p => p.TotalStock)
            .ThenBy(p => p.ProductName)];
    }
}