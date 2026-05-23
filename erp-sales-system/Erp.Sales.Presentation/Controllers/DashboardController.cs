using Erp.Sales.Application.UseCases.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Sales.Presentation.Controllers;

[ApiController]
[Route("api/sales/dashboard")]
[ApiExplorerSettings(IgnoreApi = true)]
public class DashboardController(
    IGetDailySalesDashboardUseCase getDailySalesDashboardUseCase,
    IGetTopProductsDashboardUseCase getTopProductsDashboardUseCase,
    IGetKdsStatusDashboardUseCase getKdsStatusDashboardUseCase) : ControllerBase
{
    [HttpGet("{companyId:int}/daily-sales")]
    public async Task<IActionResult> GetDailySales(int companyId)
    {
        try
        {
            return Ok(await getDailySalesDashboardUseCase.ExecuteAsync(companyId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{companyId:int}/top-products")]
    public async Task<IActionResult> GetTopProducts(int companyId, [FromQuery] int topN = 10)
    {
        try
        {
            return Ok(await getTopProductsDashboardUseCase.ExecuteAsync(companyId, topN));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{companyId:int}/kds-status")]
    public async Task<IActionResult> GetKdsStatus(int companyId)
    {
        try
        {
            return Ok(await getKdsStatusDashboardUseCase.ExecuteAsync(companyId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}