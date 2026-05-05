using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Domain.Enums;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public class CancelRestaurantOrderUseCase(IRestaurantOrderRepository restaurantOrderRepository) : ICancelRestaurantOrderUseCase
{
    public async Task ExecuteAsync(CancelRestaurantOrderDto cancelRestaurantOrderDto)
    {
        var restaurantOrder = await restaurantOrderRepository.GetByIdAsync(cancelRestaurantOrderDto.RestaurantOrderId)
            ?? throw new KeyNotFoundException("La orden del restaurante no existe");

        if (restaurantOrder.Order.Status == OrderStatus.Paid)
        {
            throw new InvalidOperationException("No se puede cancelar una orden pagada");
        }

        if (restaurantOrder.Order.Status == OrderStatus.Cancelled)
        {
            return;
        }

        await restaurantOrderRepository.CancelOrderAsync(cancelRestaurantOrderDto.RestaurantOrderId);
    }
}
