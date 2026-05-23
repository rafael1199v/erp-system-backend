namespace Erp.Sales.Application.Interfaces;

public sealed record SalesCenLookup(int Id, string Cen, string? Name = null);

public interface ISalesCenResolver
{
    Task<int?> ResolveCompanyIdAsync(string companyCen);
    Task<SalesCenLookup?> ResolveTicketAsync(string companyCen, string ticketCen);
    Task<SalesCenLookup?> ResolveTicketItemAsync(string companyCen, string ticketCen, string ticketItemCen);
    Task<SalesCenLookup?> ResolveWaiterAsync(string companyCen, string waiterCen);
    Task<SalesCenLookup?> ResolveTeamAsync(string companyCen, string teamCen);
    Task<SalesCenLookup?> ResolveSaleAsync(string companyCen, string saleCen);
    Task<int?> ResolveMainWarehouseIdAsync(string companyCen, string warehouseCen);
}
