using Erp.Sales.Application.DTOs;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public interface IProcessRestaurantOrderPaymentUseCase
{
    Task<ProcessRestaurantOrderPaymentResultDto> ExecuteAsync(ProcessRestaurantOrderPaymentDto processRestaurantOrderPaymentDto);
}
