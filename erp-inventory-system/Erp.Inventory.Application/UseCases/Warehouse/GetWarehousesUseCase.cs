using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Application.Mappers;

namespace Erp.Inventory.Application.UseCases.Warehouse;

public class GetWarehousesUseCase : IGetWarehousesUseCase
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IWarehouseMapper _warehouseMapper;
    
    
    public GetWarehousesUseCase(IWarehouseRepository warehouseRepository, IWarehouseMapper warehouseMapper)
    {
        _warehouseRepository = warehouseRepository;
        _warehouseMapper = warehouseMapper;
    }
    
    public async Task<List<WarehouseDTO>> GetWarehousesByCompany(int companyId)
    {
        var warehouseEntities = await _warehouseRepository.GetWarehousesByCompanyAsync(companyId);
        return warehouseEntities.Select(_warehouseMapper.EntityToWarehouseDto).ToList();
    }
}