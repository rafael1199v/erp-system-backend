namespace Erp.Sales.Application.UseCases.TaxConfiguration;

public interface IGetGlobalTaxUseCase
{
    Task<decimal> ExecuteAsync(int companyId);
}