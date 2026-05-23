using Erp.Sales.Application.ContractServices;
using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Application.UseCases.Dashboard;
using Erp.Sales.Application.ContractDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Sales.Presentation.Controllers.Contract;

[ApiController]
[Route("api/sales/companies/{companyCen}/dashboard")]
public class DashboardContractController(
    ISalesCenResolver salesCenResolver,
    ISalesDashboardContractService salesDashboardContractService,
    IGetDailySalesDashboardUseCase getDailySalesDashboardUseCase,
    IGetKdsStatusDashboardUseCase getKdsStatusDashboardUseCase)
    : ControllerBase
{
    private const int DefaultTopN = 10;

    [EndpointSummary("Obtiene ventas diarias")]
    [EndpointDescription("""
                         Devuelve el resumen de ventas del dia actual para la empresa.
                         Usar en dashboards ejecutivos de ventas.
                         """)]
    [ProducesResponseType(typeof(DailySalesDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [HttpGet("daily-sales")]
    public async Task<IActionResult> GetDailySales(string companyCen)
    {
        int? companyId = await ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada" });
        }

        return Ok(await getDailySalesDashboardUseCase.ExecuteAsync(companyId.Value));
    }

    [EndpointSummary("Obtiene top productos vendidos")]
    [EndpointDescription("""
                         Devuelve los productos mas vendidos del dia actual.
                         Usar para analitica rapida y reportes de desempeño.
                         Integra con el API de Inventario para enriquecer datos de producto.
                         """)]
    [ProducesResponseType(typeof(List<TopProductDashboardContractResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [HttpGet("top-products")]
    public async Task<IActionResult> GetTopProducts(string companyCen, [FromQuery] int topN = DefaultTopN)
    {
        int? companyId = await ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada" });
        }

        return Ok(await salesDashboardContractService.GetTopProductsAsync(companyId.Value, companyCen, topN));
    }

    [EndpointSummary("Obtiene estado del KDS")]
    [EndpointDescription("""
                         Devuelve el estado operativo del sistema KDS para la empresa.
                         Usar para indicadores de cocina o tableros de servicio.
                         """)]
    [ProducesResponseType(typeof(KdsStatusDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [HttpGet("kds-status")]
    public async Task<IActionResult> GetKdsStatus(string companyCen)
    {
        int? companyId = await ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada" });
        }

        return Ok(await getKdsStatusDashboardUseCase.ExecuteAsync(companyId.Value));
    }

    private async Task<int?> ResolveCompanyIdAsync(string companyCen)
    {
        return await salesCenResolver.ResolveCompanyIdAsync(companyCen);
    }
}
