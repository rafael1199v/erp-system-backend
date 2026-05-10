using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Purchasing.Presentation.Controllers;

[ApiController]
[Route("api/purchases/companies/{companyCen}/suppliers")]
public class SupplierController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSuppliers(string companyCen)
    {
        return Ok();
    }   
}