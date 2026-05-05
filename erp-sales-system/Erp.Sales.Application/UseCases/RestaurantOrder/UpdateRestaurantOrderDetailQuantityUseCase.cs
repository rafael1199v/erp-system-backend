using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public class UpdateRestaurantOrderDetailQuantityUseCase(
    IRestaurantOrderDetailRepository restaurantOrderDetailRepository) : IUpdateRestaurantOrderDetailQuantityUseCase
{
    public async Task ExecuteAsync(UpdateRestaurantOrderDetailQuantityDto updateRestaurantOrderDetailQuantityDto)
    {
        var restaurantOrderDetail = await restaurantOrderDetailRepository
            .GetByIdAsync(updateRestaurantOrderDetailQuantityDto.RestaurantOrderDetailId) ?? throw new KeyNotFoundException("El detalle no existe");

        if (!await restaurantOrderDetailRepository
                .RestaurantOrderExistsAndOpenAsync(restaurantOrderDetail.RestaurantOrderId))
        {
            throw new InvalidOperationException("La orden del restaurante no esta abierta");
        }

        restaurantOrderDetail.UpdateQuantity(updateRestaurantOrderDetailQuantityDto.Quantity);
        restaurantOrderDetail.UpdateNote(updateRestaurantOrderDetailQuantityDto.Note);

        await restaurantOrderDetailRepository.UpdateQuantityAsync(
            restaurantOrderDetail.Id,
            restaurantOrderDetail.Quantity,
            restaurantOrderDetail.Note);
    }
}