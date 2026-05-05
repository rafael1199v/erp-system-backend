using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.Interfaces;

public interface IWarehouseRepository
{
    Task<WarehouseEntity> GetWarehouseByIdAsync(int warehouseId);
    Task<List<WarehouseEntity>> GetWarehousesByCompanyAsync(int companyId);
}