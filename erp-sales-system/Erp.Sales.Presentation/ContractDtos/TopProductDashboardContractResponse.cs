namespace Erp.Sales.Presentation.ContractDtos;

public class TopProductDashboardContractResponse
{
    public string? ProductCen { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
    public string? CategoryCen { get; set; }
    public string? CategoryName { get; set; }
    public decimal SalePrice { get; set; }
}
