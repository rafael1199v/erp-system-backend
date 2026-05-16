namespace Erp.Inventory.Application.ContractDtos;

public class CreateProductContractResponse
{
    public string ProductCen { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal InitialStock { get; set; }
}
