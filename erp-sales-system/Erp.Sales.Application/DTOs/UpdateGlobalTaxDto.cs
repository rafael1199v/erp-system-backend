namespace Erp.Sales.Application.DTOs;

public record UpdateGlobalTaxDto(
    int CompanyId,    
    decimal GlobalTaxPercentage
);