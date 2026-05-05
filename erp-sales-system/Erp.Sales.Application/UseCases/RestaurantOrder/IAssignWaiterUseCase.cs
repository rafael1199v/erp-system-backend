using Erp.Sales.Application.DTOs;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public interface IAssignWaiterUseCase
{
    Task ExecuteAsync(AssignWaiterDto assignWaiterDto);
}