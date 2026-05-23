using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.Mappers;

public class WarehouseMapper : IWarehouseMapper
{
    public WarehouseDTO EntityToWarehouseDto(WarehouseEntity warehouseEntity)
    {
        return new WarehouseDTO
        {
            Id = warehouseEntity.Id,
            Cen = warehouseEntity.Cen,
            Name = warehouseEntity.Name
        };
    }
}
