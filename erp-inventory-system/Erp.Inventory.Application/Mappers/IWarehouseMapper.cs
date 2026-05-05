using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.Mappers;

public interface IWarehouseMapper
{
    WarehouseDTO EntityToWarehouseDto(WarehouseEntity warehouseEntity);
}