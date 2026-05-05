using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public class AssignWaiterUseCase(IRestaurantOrderRepository restaurantOrderRepository) : IAssignWaiterUseCase
{
    public async Task ExecuteAsync(AssignWaiterDto assignWaiterDto)
    {
        await restaurantOrderRepository.AssignWaiter(assignWaiterDto.RestaurantOrderId, assignWaiterDto.WaiterId);
    }
}