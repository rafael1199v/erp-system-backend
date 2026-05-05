using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Domain.Entities;
using Erp.Inventory.Infrastructure.Persistance.Context;
using Erp.Inventory.Infrastructure.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace Erp.Inventory.Infrastructure.Persistance.Repositories;

public class SupplierRepository(AppDbContext context) : ISupplierRepository
{
    public async Task<List<SupplierEntity>> GetAllByCompany(int companyId)
    {
        var suppliers = await Queryable.Where<Supplier>(context.Suppliers, s => s.CompanyId == companyId).ToListAsync();

        return Enumerable.ToList(suppliers.Select(ToDomain));
    }

    private static SupplierEntity ToDomain(Supplier supplier)
    {
        return new SupplierEntity
        {
            Id = supplier.Id,
            Name = supplier.Name,
        };
    }
}