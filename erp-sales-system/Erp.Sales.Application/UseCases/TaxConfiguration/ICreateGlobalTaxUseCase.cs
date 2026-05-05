using Erp.Sales.Application.DTOs;

namespace Erp.Sales.Application.UseCases.TaxConfiguration;

public interface ICreateGlobalTaxUseCase
{
    Task ExecuteAsync(CreateGlobalTaxDto createGlobalTaxDto);
}