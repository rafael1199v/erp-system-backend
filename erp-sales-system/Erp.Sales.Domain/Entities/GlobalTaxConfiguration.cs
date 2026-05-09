namespace Erp.Sales.Domain.Entities;

public class GlobalTaxConfiguration
{
    public required int CompanyId { get; init; }
    public string? CompanyCen { get; init; }
    public required decimal GlobalTaxPercentage { get; init; }


    public static GlobalTaxConfiguration Create(int companyId, decimal globalTaxPercentage, string? companyCen = null)
    {
        if (globalTaxPercentage is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(globalTaxPercentage), "El porcentaje global del impuesto debe estar entre 0 y 100.");
        }

        return new GlobalTaxConfiguration
        {
            CompanyId = companyId,
            CompanyCen = companyCen,
            GlobalTaxPercentage = globalTaxPercentage
        };
    }
}
