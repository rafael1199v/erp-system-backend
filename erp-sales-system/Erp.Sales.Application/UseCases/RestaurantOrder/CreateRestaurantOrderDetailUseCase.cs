using Erp.Inventory.Contracts;
using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Domain.Entities;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public class CreateRestaurantOrderDetailUseCase(
	IRestaurantOrderDetailRepository restaurantOrderDetailRepository,
    IRestaurantOrderRepository restaurantOrderRepository,
	IWarehouseConfigurationRepository warehouseConfigurationRepository,
	IInventoryService inventoryService) : ICreateRestaurantOrderDetailUseCase
{
	public async Task<int> ExecuteAsync(CreateRestaurantOrderDetail createRestaurantOrderDetail)
	{
		if (!await restaurantOrderDetailRepository
				.RestaurantOrderExistsAndOpenAsync(createRestaurantOrderDetail.RestaurantOrderId))
		{
			throw new InvalidOperationException("La orden del restaurante no existe o no esta abierto.");
		}

        var companyId = await restaurantOrderRepository.GetCompanyId(createRestaurantOrderDetail.RestaurantOrderId);
        
        if(companyId == null)
        {
            throw new InvalidOperationException("Hubo un error al crear la orden. Vuelve a intentarlo");
        }

		if (!await inventoryService.IsProductActiveAsync(createRestaurantOrderDetail.ProductId, companyId ?? -1))
		{
			throw new InvalidOperationException("El producto esta inactivo");
		}

    
		var warehouseId = await warehouseConfigurationRepository.GetWarehouseIdByCompanyIdAsync(companyId ?? -1);
		
		if (!await inventoryService.HasAvailableStockAsync(
				createRestaurantOrderDetail.ProductId,
				createRestaurantOrderDetail.Quantity,
                companyId ?? -1, warehouseId ?? -1))
		{
			throw new InvalidOperationException("El producto no tiene stock disponible");
		}

		var restaurantOrderDetail = RestaurantOrderDetail.Create(
			restaurantOrderId: createRestaurantOrderDetail.RestaurantOrderId,
			productId: createRestaurantOrderDetail.ProductId,
			quantity: createRestaurantOrderDetail.Quantity,
			note: createRestaurantOrderDetail.Note
		);

		return await restaurantOrderDetailRepository.CreateAsync(restaurantOrderDetail);
	}
}