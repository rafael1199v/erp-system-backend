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

    public async Task<int?> ResolveIdByCodeAsync(string paymentMethodCode)
    {
        string normalizedCode = NormalizePaymentMethodCode(paymentMethodCode);
        if (int.TryParse(normalizedCode, out int paymentTypeId)
            && await ExistsAsync(paymentTypeId))
        {
            return paymentTypeId;
        }

        var paymentTypes = await salesDbContext.PaymentTypes
            .AsNoTracking()
            .Where(pt => !pt.IsDeleted)
            .Select(pt => new { pt.Id, pt.Name })
            .ToListAsync();

        return paymentTypes
            .Where(paymentType => NormalizePaymentMethodCode(paymentType.Name) == normalizedCode)
            .Select(paymentType => (int?)paymentType.Id)
            .FirstOrDefault();
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

    private static string NormalizePaymentMethodCode(string value)
    {
        return value
            .Trim()
            .Replace(" ", "_")
            .Replace("-", "_")
            .ToUpperInvariant();
    }
}
