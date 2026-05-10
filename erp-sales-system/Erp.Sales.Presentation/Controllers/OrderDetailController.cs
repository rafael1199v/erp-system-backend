using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.UseCases.RestaurantOrder;
using Erp.Sales.Application.UseCases.RestaurantOrderDetails;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Sales.Presentation.Controllers;

[ApiController]
[Route("api/sales/order-detail")]
[ApiExplorerSettings(IgnoreApi = true)]
public class OrderDetailController(
    ICreateRestaurantOrderDetailUseCase createRestaurantOrderDetailUseCase,
    IUpdateRestaurantOrderDetailQuantityUseCase updateRestaurantOrderDetailQuantityUseCase,
    IGetOrderDetailProductsUseCase getOrderDetailProductsUseCase,
    IResendOrderDetailUseCase resendOrderDetailUseCase) : ControllerBase
{

    [HttpGet("products/{restaurantOrderId:int}")]
    public async Task<IActionResult> GetProducts(int restaurantOrderId)
    {
        try
        {
            return Ok(await getOrderDetailProductsUseCase.ExecuteAsync(restaurantOrderId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrderDetail([FromBody] CreateRestaurantOrderDetail restaurantOrderDetail)
    {
        try
        {
            var restaurantOrderDetailId = await createRestaurantOrderDetailUseCase.ExecuteAsync(restaurantOrderDetail);
            return Ok(new { restaurantOrderDetailId });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateQuantity(
        [FromBody] UpdateRestaurantOrderDetailQuantityDto updateRestaurantOrderDetailQuantityDto)
    {
        try
        {
            await updateRestaurantOrderDetailQuantityUseCase.ExecuteAsync(updateRestaurantOrderDetailQuantityDto);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{restaurantOrderDetailId:int}")]
    public async Task<IActionResult> ResendOrderDetail(int restaurantOrderDetailId)
    {
        try
        {
            await resendOrderDetailUseCase.ExecuteAsync(restaurantOrderDetailId);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}