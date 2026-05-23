using Erp.Sales.Application.ContractDtos;
using Erp.Sales.Domain.Entities;

namespace Erp.Sales.Application.ContractServices;

public interface ITicketContractService
{
    TicketContractResponse ToTicketResponse(RestaurantOrder ticket);

    Task<List<TicketItemContractResponse>> ToTicketItemResponsesAsync(
        string companyCen,
        List<RestaurantOrderDetail> details);
}
