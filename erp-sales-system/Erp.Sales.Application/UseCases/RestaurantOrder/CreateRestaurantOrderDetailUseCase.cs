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

        var restaurantOrder = await restaurantOrderRepository.GetByIdAsync(createRestaurantOrderDetail.RestaurantOrderId);
        if (restaurantOrder is null)
        {
            throw new InvalidOperationException("Hubo un error al crear la orden. Vuelve a intentarlo");
        }

        string? companyCen = Normalize(restaurantOrder.CompanyCen);
        string? productCen = Normalize(createRestaurantOrderDetail.ProductCen);
        string? warehouseCen = Normalize(await warehouseConfigurationRepository
            .GetWarehouseCenByCompanyIdAsync(restaurantOrder.CompanyId));

        if (productCen is not null)
        {
            if (!CanUseCenInventory(companyCen, productCen, warehouseCen))
            {
                throw new InvalidOperationException("No hay datos CEN suficientes para validar el producto");
            }

            await ValidateProductWithCenContractAsync(
                companyCen!,
                productCen!,
                warehouseCen!,
                createRestaurantOrderDetail.Quantity);
        }
        else
        {
            if (createRestaurantOrderDetail.ProductId <= 0)
            {
                throw new InvalidOperationException("El producto es requerido");
            }

            await ValidateProductWithLegacyContractAsync(createRestaurantOrderDetail, restaurantOrder.CompanyId);
        }

		var restaurantOrderDetail = RestaurantOrderDetail.Create(
			restaurantOrderId: createRestaurantOrderDetail.RestaurantOrderId,
			productId: createRestaurantOrderDetail.ProductId,
			quantity: createRestaurantOrderDetail.Quantity,
			note: createRestaurantOrderDetail.Note,
            productCen: productCen
		);

		return await restaurantOrderDetailRepository.CreateAsync(restaurantOrderDetail);
	}

    private async Task ValidateProductWithCenContractAsync(
        string companyCen,
        string productCen,
        string warehouseCen,
        int quantity)
    {
        var products = await inventoryService.GetProductsAsync(companyCen, search: productCen);
        var product = products.FirstOrDefault(candidate =>
            string.Equals(candidate.ProductCen, productCen, StringComparison.OrdinalIgnoreCase));

        if (product is null)
        {
            throw new InvalidOperationException("El producto no existe");
        }

        if (!string.Equals(product.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("El producto esta inactivo");
        }

        var stockValidation = await inventoryService.ValidateStockAsync(companyCen, new StockValidationContractRequest
        {
            WarehouseCen = warehouseCen,
            Source = "SALES",
            Items =
            [
                new StockValidationItemContractDto
                {
                    ProductCen = productCen,
                    Quantity = quantity
                }
            ]
        });

        if (!stockValidation.IsValid)
        {
            throw new InvalidOperationException("El producto no tiene stock disponible");
        }
    }

    private async Task ValidateProductWithLegacyContractAsync(
        CreateRestaurantOrderDetail createRestaurantOrderDetail,
        int companyId)
    {
        if (!await inventoryService.IsProductActiveAsync(createRestaurantOrderDetail.ProductId, companyId))
        {
            throw new InvalidOperationException("El producto esta inactivo");
        }

        var warehouseId = await warehouseConfigurationRepository.GetWarehouseIdByCompanyIdAsync(companyId);

        if (!await inventoryService.HasAvailableStockAsync(
                createRestaurantOrderDetail.ProductId,
                createRestaurantOrderDetail.Quantity,
                companyId,
                warehouseId ?? -1))
        {
            throw new InvalidOperationException("El producto no tiene stock disponible");
        }
    }

    private static bool CanUseCenInventory(string? companyCen, string? productCen, string? warehouseCen)
    {
        return companyCen is not null && productCen is not null && warehouseCen is not null;
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
