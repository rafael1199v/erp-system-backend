using Erp.Inventory.Application.UseCases.Warehouse;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers;

[ApiController]
[Route("api/inventory/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class WarehouseController : ControllerBase
{
    private readonly IGetWarehousesUseCase _getWarehousesUseCase;

    public WarehouseController(IGetWarehousesUseCase getWarehouseUseCase)
    {
        _getWarehousesUseCase = getWarehouseUseCase;
    }
    
    [HttpGet("{companyId:int}")]
    public async Task<IActionResult> GetWarehouses(int companyId)
    {
        try
        {
            return Ok(await _getWarehousesUseCase.GetWarehousesByCompany(companyId));
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
}