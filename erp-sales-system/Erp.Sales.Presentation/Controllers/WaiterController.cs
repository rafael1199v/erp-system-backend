using Erp.Sales.Application.UseCases.Waiters;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Sales.Presentation.Controllers;

[ApiController]
[Route("api/sales/[controller]")]
public class WaiterController(IGetWaitersByCompanyUseCase getWaitersByCompanyUseCase) : ControllerBase
{
    [HttpGet("{companyId:int}")]
    public async Task<IActionResult> GetWaitersByCompanyAsync(int companyId)
    {
        try
        {
            return Ok(await getWaitersByCompanyUseCase.ExecuteAsync(companyId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}