using Erp.Inventory.Contracts;
using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;

namespace Erp.Sales.Application.UseCases.KDS;

public class GetKdsTeamItemsUseCase(IKdsRepository kdsRepository, IInventoryService inventoryService)
    : IGetKdsTeamItemsUseCase
{
    public async Task<List<KdsOrderItem>> ExecuteAsync(int companyId, int teamId)
    {
        var team = await kdsRepository.GetTeamByIdAsync(companyId, teamId);
        if (team is null)
        {
            throw new InvalidOperationException("El equipo KDS no existe para la compania indicada.");
        }

        if (team.CategoryIds.Count == 0)
        {
            return [];
        }

        var fromUtc = DateTime.UtcNow.AddHours(-48);
        var orderItems = await kdsRepository.GetOrderItemsByCompanyIdAsync(companyId, fromUtc);
        if (orderItems.Count == 0)
        {
            return [];
        }

        var productIds = orderItems.Select(oi => oi.ProductId).Distinct().ToList();
        var products = await inventoryService.GetOrderDetailProductsByIdsAsync(productIds);
        var productMap = products.ToDictionary(p => p.ProductId, p => p);
        var teamCategories = team.CategoryIds.ToHashSet();

        return [.. orderItems
            .Select(item =>
            {
                if (!productMap.TryGetValue(item.ProductId, out var product))
                {
                    return null;
                }

                if (!teamCategories.Contains(product.CategoryId))
                {
                    return null;
                }

                return new KdsOrderItem(
                    ProductId: item.ProductId,
                    CategoryId: product.CategoryId,
                    RestaurantOrderDetailId: item.RestaurantOrderDetailId,
                    RestaurantOrderId: item.RestaurantOrderId,
                    ProductName: product.Name,
                    Quantity: item.Quantity,
                    OrderItemStatus: item.RestaurantOrderDetailStatus.ToString(),
                    OrderItemStatusId: (int)item.RestaurantOrderDetailStatus,
                    Note: item.Note,
                    ResendCount: item.ResendCount);
            })
            .Where(item => item is not null)
            .Select(item => item!)];
    }

}
