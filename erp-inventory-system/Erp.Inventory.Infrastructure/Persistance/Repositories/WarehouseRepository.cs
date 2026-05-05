using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Domain.Entities;
using Erp.Inventory.Infrastructure.Persistance.Context;
using Erp.Inventory.Infrastructure.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace Erp.Inventory.Infrastructure.Persistance.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly AppDbContext _context;

    public WarehouseRepository(AppDbContext context)
    {
        this._context = context; 
    }
    
    public async Task<WarehouseEntity> GetWarehouseByIdAsync(int warehouseId)
    {
        var warehouse = await _context.Warehouses.FindAsync(warehouseId);
        return warehouse != null ? this.ToDomainEntity(warehouse) : throw new Exception("Warehouse not found");
    }

    public async Task<List<WarehouseEntity>> GetWarehousesByCompanyAsync(int companyId)
    {
        var warehouses = await Queryable.Where<Warehouse>(_context.Warehouses, w => w.CompanyId == companyId).ToListAsync();
        return Enumerable.ToList(warehouses.Select(ToDomainEntity));
    }


    private WarehouseEntity ToDomainEntity(Warehouse warehouseModel)
    {
        return new WarehouseEntity
        {
            Id = warehouseModel.Id,
            Cen = warehouseModel.Cen,
            Name = warehouseModel.Name,
        };
    }
}
