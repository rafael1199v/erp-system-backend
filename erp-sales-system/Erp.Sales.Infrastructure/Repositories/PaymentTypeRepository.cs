using Erp.Sales.Application.Interfaces;
using Erp.Sales.Domain.Entities;
using Erp.Sales.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Erp.Sales.Infrastructure.Repositories;

public class PaymentTypeRepository(SalesDbContext salesDbContext) : IPaymentTypeRepository
{
    public async Task<bool> ExistsAsync(int paymentTypeId)
    {
        return await salesDbContext.PaymentTypes
            .AnyAsync(pt => pt.Id == paymentTypeId && !pt.IsDeleted);
    }

    public async Task<List<PaymentTypeOption>> GetAllAsync()
    {
        return await Queryable
            .Where<PaymentTypeModel>(salesDbContext.PaymentTypes, pt => !pt.IsDeleted)
            .OrderBy(pt => pt.Id)
            .Select(pt => new PaymentTypeOption
            {
                Id = pt.Id,
                Name = pt.Name
            })
            .ToListAsync();
    }
}
