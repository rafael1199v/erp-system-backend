using Erp.Inventory.Application.UseCases.Supplier;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers;

[ApiController]
[Route("api/inventory/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class SupplierController(IGetSuppliersByCompanyUseCase getSuppliersByCompanyUseCase) : ControllerBase
{
    [HttpGet("{companyId}")]
    public async Task<IActionResult> GetSuppliersByCompany(int companyId)
    {
        try
        {
            return Ok(await getSuppliersByCompanyUseCase.ExecuteAsync(companyId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}