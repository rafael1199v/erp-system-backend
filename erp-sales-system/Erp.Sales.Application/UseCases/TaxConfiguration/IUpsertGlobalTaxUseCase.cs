using Erp.Sales.Application.DTOs;

namespace Erp.Sales.Application.UseCases.TaxConfiguration;

public interface IUpsertGlobalTaxUseCase
{
    Task<decimal> ExecuteAsync(UpsertGlobalTaxDto upsertGlobalTaxDto);
}
