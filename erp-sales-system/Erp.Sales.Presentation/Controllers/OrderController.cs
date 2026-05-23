using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.UseCases.RestaurantOrder;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Sales.Presentation.Controllers;

[ApiController]
[Route("api/sales/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class OrderController(ICreateRestaurantOrderUseCase createRestaurantOrderUseCase,
    IGetRestaurantOrdersUseCase getRestaurantOrdersUseCase,
    IAssignWaiterUseCase assignWaiterUseCase,
    ICancelRestaurantOrderUseCase cancelRestaurantOrderUseCase,
    IGetRestaurantOrderDetailsUseCase getRestaurantOrderDetailsUseCase,
    ISendOrderUseCase sendOrderUseCase,
    IGetRestaurantOrderTaxUseCase getRestaurantOrderTaxUseCase,
    IPrintRestaurantOrderUseCase printRestaurantOrderUseCase) : ControllerBase
{
    
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateRestaurantOrderDto createRestaurantOrderDto)
    {
        try
        {
            var restaurantOrderId = await createRestaurantOrderUseCase.ExecuteAsync(createRestaurantOrderDto);
            return Ok(new { restaurantOrderId });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{companyId:int}")]
    public async Task<IActionResult> GetDailyOrders(int companyId)
    {
        try
        {
            return Ok(await getRestaurantOrdersUseCase.ExecuteAsync(companyId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("details/{restaurantOrderId:int}")]
    public async Task<IActionResult> GetRestaurantOrderDetails(int restaurantOrderId)
    {
        try
        {
            return Ok(await getRestaurantOrderDetailsUseCase.ExecuteAsync(restaurantOrderId));
        }   
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPatch("details/{restaurantOrderId:int}")]
    public async Task<IActionResult> SendOrderToTeams(int restaurantOrderId)
    {
        try
        {
            await sendOrderUseCase.ExecuteAsync(restaurantOrderId);
            return Ok();
        }   
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
 

    [HttpPut("assign")]
    public async Task<IActionResult> AssignWaiter([FromBody] AssignWaiterDto assignWaiterDto)
    {
        try
        {
            await assignWaiterUseCase.ExecuteAsync(assignWaiterDto);
            return Ok();
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("cancel")]
    public async Task<IActionResult> CancelRestaurantOrder([FromBody] CancelRestaurantOrderDto cancelRestaurantOrderDto)
    {
        try
        {
            await cancelRestaurantOrderUseCase.ExecuteAsync(cancelRestaurantOrderDto);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet("tax/{restaurantOrderId:int}")]
    public async Task<IActionResult> GetOrderTax(int restaurantOrderId)
    {
        try
        {
            return Ok(await getRestaurantOrderTaxUseCase.ExecuteAsync(restaurantOrderId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{restaurantOrderId:int}/print")]
    public async Task<IActionResult> PrintRestaurantOrder(int restaurantOrderId)
    {
        try
        {
            return File(await printRestaurantOrderUseCase.ExecuteAsync(restaurantOrderId), "application/pdf", $"orden-{restaurantOrderId}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
}