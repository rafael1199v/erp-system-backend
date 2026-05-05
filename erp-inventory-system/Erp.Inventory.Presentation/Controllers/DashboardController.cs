using Erp.Inventory.Application.UseCases.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers;

[ApiController]
[Route("api/inventory/dashboard")]
public class DashboardController(IGetLowStockDashboardUseCase getLowStockDashboardUseCase) : ControllerBase
{
    [HttpGet("{companyId:int}/low-stock")]
    public async Task<IActionResult> GetLowStock(int companyId)
    {
        try
        {
            return Ok(await getLowStockDashboardUseCase.ExecuteAsync(companyId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}