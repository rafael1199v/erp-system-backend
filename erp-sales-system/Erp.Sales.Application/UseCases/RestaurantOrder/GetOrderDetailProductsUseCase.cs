using Erp.Inventory.Contracts;
using Erp.Sales.Application.Interfaces;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public class GetOrderDetailProductsUseCase(
    IRestaurantOrderDetailRepository restaurantOrderDetailRepository,
    IRestaurantOrderRepository restaurantOrderRepository,
    IWarehouseConfigurationRepository warehouseConfigurationRepository,
    IInventoryService inventoryService) : IGetOrderDetailProductsUseCase
{
    public async Task<List<RestaurantOrderProductDto>> ExecuteAsync(int restaurantOrderId)
    {
        if (!await restaurantOrderDetailRepository.RestaurantOrderExistsAndOpenAsync(restaurantOrderId))
        {
            throw new InvalidOperationException("La orden del restaurante no existe o no esta abierta.");
        }

        int? companyId = await restaurantOrderRepository.GetCompanyId(restaurantOrderId);
        if (companyId is null)
        {
            throw new InvalidOperationException("No se pudo obtener la compania asociada a la orden.");
        }

        int? warehouseId = await warehouseConfigurationRepository.GetWarehouseIdByCompanyIdAsync(companyId.Value);
        if (warehouseId is null)
        {
            throw new InvalidOperationException("No hay una bodega principal configurada para la compania.");
        }

        return await inventoryService.GetOrderDetailProductsAsync(companyId.Value, warehouseId.Value);
    }
}
