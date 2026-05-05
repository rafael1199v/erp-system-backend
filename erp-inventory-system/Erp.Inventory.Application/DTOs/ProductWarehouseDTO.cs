namespace Erp.Inventory.Application.DTOs;

public class ProductWarehouseDTO
{
    public GetProductCatalogDTO Product { get; set; } = null!;
    public List<GetWarehouseWithStockDTO> Warehouses { get; set; } = new List<GetWarehouseWithStockDTO>();
}