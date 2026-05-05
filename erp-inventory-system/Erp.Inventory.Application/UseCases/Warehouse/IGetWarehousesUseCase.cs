using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Warehouse;

public interface IGetWarehousesUseCase
{
    Task<List<WarehouseDTO>> GetWarehousesByCompany(int companyId);
}