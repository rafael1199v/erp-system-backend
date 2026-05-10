namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public interface IPrintTicketContractUseCase
{
    Task<byte[]> ExecuteAsync(string companyCen, string ticketCen);
}
