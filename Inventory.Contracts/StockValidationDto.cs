namespace Erp.Inventory.Contracts;

public class StockValidationDto
{
    public required List<StockRequirementDto> Requirements { get; set; }
    public required int CompanyId { get; set; }
}