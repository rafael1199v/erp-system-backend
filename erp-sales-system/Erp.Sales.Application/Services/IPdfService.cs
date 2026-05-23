using Erp.Sales.Application.DTOs;

namespace Erp.Sales.Application.Services;

public interface IPdfService
{
    public byte[] GenerateRestaurantOrderPdf(int restaurantOrderId, List<RestaurantOrderDetailDto> restaurantOrderDetails);
    public byte[] GenerateTicketContractPdf(TicketPrintDto ticket);
}
