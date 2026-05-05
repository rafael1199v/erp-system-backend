using Erp.Sales.Application.Interfaces;
using Erp.Sales.Domain.Entities;
using Erp.Sales.Infrastructure.Models.PoS;
using Microsoft.EntityFrameworkCore;

namespace Erp.Sales.Infrastructure.Repositories;

public class WaiterRepository(SalesDbContext salesDbContext) : IWaiterRepository
{
    public async Task<List<Waiter>> GetWaitersByCompanyAsync(int companyId)
    {
        var waiters = await Queryable.Where<WaiterModel>(salesDbContext.Waiters, w => w.CompanyId == companyId && !w.IsDeleted).ToListAsync();

        return Enumerable.ToList(waiters.Select(ToDomain));
    }


    private static Waiter ToDomain(WaiterModel model)
    {
        return new Waiter
        {
            Id = model.Id,
            Name = model.Name,
            CompanyId = model.CompanyId
        };
    }
}