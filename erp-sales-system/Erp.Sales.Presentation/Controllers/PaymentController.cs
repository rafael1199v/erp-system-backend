using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.UseCases.RestaurantOrder;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Sales.Presentation.Controllers;

[ApiController]
[Route("api/sales/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class PaymentController(
    IGetPaymentTypesUseCase getPaymentTypesUseCase,
    IProcessRestaurantOrderPaymentUseCase processRestaurantOrderPaymentUseCase) : ControllerBase
{
    [HttpGet("methods")]
    public async Task<IActionResult> GetPaymentMethods()
    {
        try
        {
            return Ok(await getPaymentTypesUseCase.ExecuteAsync());
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("process")]
    public async Task<IActionResult> ProcessRestaurantOrderPayment([FromBody] ProcessRestaurantOrderPaymentDto processRestaurantOrderPaymentDto)
    {
        try
        {
            var result = await processRestaurantOrderPaymentUseCase.ExecuteAsync(processRestaurantOrderPaymentDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(new
            {
                saleId = result.SaleId,
                saleCen = result.SaleCen,
                inventoryDocumentCen = result.InventoryDocumentCen
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
