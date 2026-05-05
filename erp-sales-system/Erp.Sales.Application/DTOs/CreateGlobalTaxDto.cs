namespace Erp.Sales.Application.DTOs;

public record CreateGlobalTaxDto(
    int CompanyId,    
    decimal GlobalTaxPercentage
);