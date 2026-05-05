using Erp.Sales.Application.Interfaces;
using Erp.Sales.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Erp.Sales.Infrastructure.Repositories;

public class OrderRepository(SalesDbContext salesDbContext) : IOrderRepository
{
    public async Task<int?> GetCompanyId(int orderId)
    {
        int? companyId = await Queryable.Where<OrderModel>(salesDbContext.Orders, o => o.Id == orderId && !o.IsDeleted).Select(o => o.CompanyId).FirstOrDefaultAsync();

        return companyId;
    }
}
