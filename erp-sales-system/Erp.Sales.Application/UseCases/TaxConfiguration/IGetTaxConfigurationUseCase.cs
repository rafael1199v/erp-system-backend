namespace Erp.Sales.Application.UseCases.TaxConfiguration;

public interface IGetTaxConfigurationUseCase
{
    Task<decimal> ExecuteAsync(int companyId);
}
