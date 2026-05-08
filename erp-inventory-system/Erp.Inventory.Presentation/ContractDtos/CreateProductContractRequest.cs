namespace Erp.Inventory.Presentation.ContractDtos;

public class CreateProductContractRequest
{
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CategoryCen { get; set; } = string.Empty;
    public string UnitCen { get; set; } = string.Empty;
    public decimal SalePrice { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal ReorderLevel { get; set; }
    public string? StationCode { get; set; }
}
