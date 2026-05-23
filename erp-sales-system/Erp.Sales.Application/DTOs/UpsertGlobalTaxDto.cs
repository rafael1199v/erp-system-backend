namespace Erp.Sales.Application.DTOs;

public record UpsertGlobalTaxDto(
    int CompanyId,
    string CompanyCen,
    decimal GlobalTaxPercentage
);
