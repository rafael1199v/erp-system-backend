using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Application.UseCases.TaxConfiguration;
using Erp.Sales.Presentation.ContractDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Sales.Presentation.Controllers.Contract;

[ApiController]
[Route("api/sales/companies/{companyCen}/tax-configuration")]
public class TaxConfigurationContractController(
    ISalesCenResolver salesCenResolver,
    IGetTaxConfigurationUseCase getTaxConfigurationUseCase,
    IUpsertGlobalTaxUseCase upsertGlobalTaxUseCase) : ControllerBase
{
    [EndpointSummary("Obtiene configuracion de impuestos")]
    [EndpointDescription("""
                         Devuelve el porcentaje global de impuesto configurado para la empresa.
                         Usar para calcular totales en ventas.
                         """)]
    [ProducesResponseType(typeof(TaxConfigurationContractResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [HttpGet]
    public async Task<IActionResult> GetTaxConfiguration(string companyCen)
    {
        int? companyId = await salesCenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada" });
        }

        decimal globalTaxPercentage = await getTaxConfigurationUseCase.ExecuteAsync(companyId.Value);
        return Ok(ToResponse(companyCen, globalTaxPercentage));
    }

    [EndpointSummary("Actualiza configuracion de impuestos")]
    [EndpointDescription("""
                         Registra o actualiza el porcentaje global de impuesto.
                         Usar cuando se cambien reglas fiscales de la empresa.
                         """)]
    [ProducesResponseType(typeof(TaxConfigurationContractResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [HttpPut]
    public async Task<IActionResult> UpdateTaxConfiguration(
        string companyCen,
        [FromBody] UpdateTaxConfigurationContractRequest request)
    {
        int? companyId = await salesCenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada" });
        }

        try
        {
            decimal globalTaxPercentage = await upsertGlobalTaxUseCase.ExecuteAsync(new UpsertGlobalTaxDto(
                CompanyId: companyId.Value,
                CompanyCen: companyCen,
                GlobalTaxPercentage: request.GlobalTaxPercentage));

            return Ok(ToResponse(companyCen, globalTaxPercentage));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private static TaxConfigurationContractResponse ToResponse(string companyCen, decimal globalTaxPercentage)
    {
        return new TaxConfigurationContractResponse
        {
            CompanyCen = companyCen,
            GlobalTaxPercentage = globalTaxPercentage
        };
    }
}
