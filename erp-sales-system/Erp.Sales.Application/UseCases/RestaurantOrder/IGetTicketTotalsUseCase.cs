using Erp.Sales.Application.DTOs;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public interface IGetTicketTotalsUseCase
{
    Task<TicketTotalsDto> ExecuteAsync(int restaurantOrderId);
}
