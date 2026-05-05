namespace Erp.Inventory.Application.DTOs;

public record CurrentStockDto(
    int ProductId,  
    int WarehouseId,
    int Quantity
);