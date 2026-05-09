using Erp.Sales.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Erp.Sales.Infrastructure.Services;

public class SalesCenResolver(SalesDbContext context) : ISalesCenResolver
{
    public async Task<int?> ResolveCompanyIdAsync(string companyCen)
    {
        string normalizedCompanyCen = Normalize(companyCen);
        if (normalizedCompanyCen.Length == 0)
        {
            return null;
        }

        return await context.Orders
                   .AsNoTracking()
                   .Where(order => order.CompanyCen == normalizedCompanyCen && !order.IsDeleted)
                   .Select(order => (int?)order.CompanyId)
                   .FirstOrDefaultAsync()
               ?? await context.TaxConfigurations
                   .AsNoTracking()
                   .Where(tax => tax.CompanyCen == normalizedCompanyCen && !tax.IsDeleted)
                   .Select(tax => (int?)tax.CompanyId)
                   .FirstOrDefaultAsync()
               ?? await context.WarehouseConfigurations
                   .AsNoTracking()
                   .Where(configuration => configuration.CompanyCen == normalizedCompanyCen && !configuration.IsDeleted)
                   .Select(configuration => (int?)configuration.CompanyId)
                   .FirstOrDefaultAsync()
               ?? await context.Waiters
                   .AsNoTracking()
                   .Where(waiter => waiter.CompanyCen == normalizedCompanyCen && !waiter.IsDeleted)
                   .Select(waiter => (int?)waiter.CompanyId)
                   .FirstOrDefaultAsync()
               ?? await context.Teams
                   .AsNoTracking()
                   .Where(team => team.CompanyCen == normalizedCompanyCen && !team.IsDeleted)
                   .Select(team => (int?)team.CompanyId)
                   .FirstOrDefaultAsync()
               ?? await context.Sales
                   .AsNoTracking()
                   .Where(sale => sale.CompanyCen == normalizedCompanyCen && !sale.IsDeleted)
                   .Select(sale => (int?)sale.CompanyId)
                   .FirstOrDefaultAsync()
               ?? await context.Customers
                   .AsNoTracking()
                   .Where(customer => customer.CompanyCen == normalizedCompanyCen && !customer.IsDeleted)
                   .Select(customer => (int?)customer.CompanyId)
                   .FirstOrDefaultAsync()
               ?? await context.TeamConfigurations
                   .AsNoTracking()
                   .Where(configuration => configuration.CompanyCen == normalizedCompanyCen)
                   .Select(configuration => (int?)configuration.CompanyId)
                   .FirstOrDefaultAsync();
    }

    public async Task<SalesCenLookup?> ResolveTicketAsync(string companyCen, string ticketCen)
    {
        string normalizedCompanyCen = Normalize(companyCen);
        string normalizedTicketCen = Normalize(ticketCen);

        return await context.RestaurantOrders
            .AsNoTracking()
            .Where(ticket =>
                ticket.Cen == normalizedTicketCen &&
                !ticket.IsDeleted &&
                !ticket.Order.IsDeleted &&
                ticket.Order.CompanyCen == normalizedCompanyCen)
            .Select(ticket => new SalesCenLookup(ticket.Id, ticket.Cen, ticket.Order.DailyNumber.ToString()))
            .FirstOrDefaultAsync();
    }

    public async Task<SalesCenLookup?> ResolveTicketItemAsync(
        string companyCen,
        string ticketCen,
        string ticketItemCen)
    {
        string normalizedCompanyCen = Normalize(companyCen);
        string normalizedTicketCen = Normalize(ticketCen);
        string normalizedTicketItemCen = Normalize(ticketItemCen);

        return await context.RestaurantOrderDetails
            .AsNoTracking()
            .Where(item =>
                item.Cen == normalizedTicketItemCen &&
                !item.IsDeleted &&
                item.RestaurantOrder.Cen == normalizedTicketCen &&
                !item.RestaurantOrder.IsDeleted &&
                !item.RestaurantOrder.Order.IsDeleted &&
                item.RestaurantOrder.Order.CompanyCen == normalizedCompanyCen)
            .Select(item => new SalesCenLookup(item.Id, item.Cen))
            .FirstOrDefaultAsync();
    }

    public async Task<SalesCenLookup?> ResolveWaiterAsync(string companyCen, string waiterCen)
    {
        string normalizedCompanyCen = Normalize(companyCen);
        string normalizedWaiterCen = Normalize(waiterCen);

        return await context.Waiters
            .AsNoTracking()
            .Where(waiter =>
                waiter.Cen == normalizedWaiterCen &&
                waiter.CompanyCen == normalizedCompanyCen &&
                !waiter.IsDeleted)
            .Select(waiter => new SalesCenLookup(waiter.Id, waiter.Cen, waiter.Name))
            .FirstOrDefaultAsync();
    }

    public async Task<SalesCenLookup?> ResolveTeamAsync(string companyCen, string teamCen)
    {
        string normalizedCompanyCen = Normalize(companyCen);
        string normalizedTeamCen = Normalize(teamCen);

        return await context.Teams
            .AsNoTracking()
            .Where(team =>
                team.Cen == normalizedTeamCen &&
                team.CompanyCen == normalizedCompanyCen &&
                !team.IsDeleted)
            .Select(team => new SalesCenLookup(team.Id, team.Cen, team.Name))
            .FirstOrDefaultAsync();
    }

    public async Task<SalesCenLookup?> ResolveSaleAsync(string companyCen, string saleCen)
    {
        string normalizedCompanyCen = Normalize(companyCen);
        string normalizedSaleCen = Normalize(saleCen);

        return await context.Sales
            .AsNoTracking()
            .Where(sale =>
                sale.Cen == normalizedSaleCen &&
                sale.CompanyCen == normalizedCompanyCen &&
                !sale.IsDeleted)
            .Select(sale => new SalesCenLookup(sale.Id, sale.Cen))
            .FirstOrDefaultAsync();
    }

    public async Task<int?> ResolveMainWarehouseIdAsync(string companyCen, string warehouseCen)
    {
        string normalizedCompanyCen = Normalize(companyCen);
        string normalizedWarehouseCen = Normalize(warehouseCen);

        return await context.WarehouseConfigurations
            .AsNoTracking()
            .Where(configuration =>
                configuration.CompanyCen == normalizedCompanyCen &&
                configuration.MainWarehouseCen == normalizedWarehouseCen &&
                !configuration.IsDeleted)
            .Select(configuration => (int?)configuration.MainWarehouseId)
            .FirstOrDefaultAsync();
    }

    private static string Normalize(string value)
    {
        return value.Trim();
    }
}
