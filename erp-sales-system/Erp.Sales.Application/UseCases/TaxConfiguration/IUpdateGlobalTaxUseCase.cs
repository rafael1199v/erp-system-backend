using Erp.Sales.Application.DTOs;

namespace Erp.Sales.Application.UseCases.TaxConfiguration;

public interface IUpdateGlobalTaxUseCase
{
    Task ExecuteAsync(UpdateGlobalTaxDto updateGlobalTaxDto);
}