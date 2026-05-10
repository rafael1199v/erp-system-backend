using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Purchasing.Presentation.Controllers;

[ApiController]
[Route("api/purchases/company/{companyCen}/orders")]
public class PurchaseOrderController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetOrders(string companyCen)
    {
        return Ok();
    }

    [HttpGet("{orderCen}")]
    public async Task<IActionResult> GetOrder(string companyCen, string orderCen)
    {
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateOrder(string companyCen)
    {
        return Ok();
    }

    [HttpPatch("{orderCen}/confirm")]
    public async Task<IActionResult> ConfirmOrder(string companyCen, string orderCen)
    {
        return Ok();
    }
}