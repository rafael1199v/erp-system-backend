using Erp.Purchasing.Application.DTOs;
using Erp.Purchasing.Application.Repositories;
using Erp.Purchasing.Domain.Entities;
using Erp.Purchasing.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Erp.Purchasing.Infrastructure.Persistence.Repositories;

public class SupplierRepository(PurchasingDbContext context) : ISupplierRepository
{
    public async Task<IReadOnlyCollection<SupplierDto>> GetByCompanyAsync(
        string companyCen,
        CancellationToken ct = default)
    {
        if (!Guid.TryParse(companyCen, out var parsedCompanyCen))
        {
            return Array.Empty<SupplierDto>();
        }

        return await context.Suppliers
            .AsNoTracking()
            .Where(supplier => supplier.CompanyCen == parsedCompanyCen && supplier.IsActive)
            .OrderBy(supplier => supplier.Name)
            .Select(supplier => new SupplierDto(
                supplier.Cen.ToString(),
                supplier.Name))
            .ToListAsync(ct);
    }

    public async Task<Supplier?> GetByCompanyAndCenAsync(
        string companyCen,
        string supplierCen,
        CancellationToken ct = default)
    {
        if (!Guid.TryParse(companyCen, out var parsedCompanyCen)
            || !Guid.TryParse(supplierCen, out var parsedSupplierCen))
        {
            return null;
        }

        return await context.Suppliers
            .AsNoTracking()
            .Where(supplier =>
                supplier.CompanyCen == parsedCompanyCen
                && supplier.Cen == parsedSupplierCen
                && supplier.IsActive)
            .Select(supplier => Supplier.Restore(
                supplier.Id,
                supplier.Cen.ToString(),
                supplier.CompanyCen.ToString(),
                supplier.Name))
            .FirstOrDefaultAsync(ct);
    }
}
