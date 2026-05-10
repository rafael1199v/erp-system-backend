using Erp.Purchasing.Application.DTOs;
using Erp.Purchasing.Application.Exceptions;
using Erp.Purchasing.Application.UseCases;
using Erp.Purchasing.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Purchasing.Presentation.Controllers;

[ApiController]
[Route("api/purchases/company/{companyCen}/orders")]
public class PurchaseOrderController(
    IGetPurchaseUseCase getPurchaseUseCase,
    ICreatePurchaseUseCase createPurchaseUseCase,
    IConfirmPurchaseUseCase confirmPurchaseUseCase) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetOrders(
        string companyCen,
        [FromQuery] PurchaseStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool sortDescending = true,
        CancellationToken ct = default)
    {
        try
        {
            var query = new PurchaseOrderQueryDto(status, page, pageSize, sortDescending);
            return Ok(await getPurchaseUseCase.GetPagedAsync(companyCen, query, ct));
        }
        catch (Exception ex)
        {
            return ToErrorResult(ex);
        }
    }

    [HttpGet("{orderCen}")]
    public async Task<IActionResult> GetOrder(string companyCen, string orderCen, CancellationToken ct = default)
    {
        try
        {
            return Ok(await getPurchaseUseCase.GetDetailAsync(companyCen, orderCen, ct));
        }
        catch (Exception ex)
        {
            return ToErrorResult(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        string companyCen,
        [FromBody] CreatePurchaseOrderDto request,
        CancellationToken ct = default)
    {
        try
        {
            var result = await createPurchaseUseCase.ExecuteAsync(companyCen, request, ct);
            return CreatedAtAction(
                nameof(GetOrder),
                new { companyCen, orderCen = result.OrderCen },
                result);
        }
        catch (Exception ex)
        {
            return ToErrorResult(ex);
        }
    }

    [HttpPost("{orderCen}/confirm")]
    public async Task<IActionResult> ConfirmOrder(string companyCen, string orderCen, CancellationToken ct = default)
    {
        try
        {
            return Ok(await confirmPurchaseUseCase.ExecuteAsync(companyCen, orderCen, ct));
        }
        catch (Exception ex)
        {
            return ToErrorResult(ex);
        }
    }

    private IActionResult ToErrorResult(Exception exception)
    {
        return exception switch
        {
            PurchasingNotFoundException => NotFound(new { message = exception.Message }),
            KeyNotFoundException => NotFound(new { message = exception.Message }),
            PurchasingBusinessException => BadRequest(new { message = exception.Message }),
            InvalidOperationException => BadRequest(new { message = exception.Message }),
            _ => BadRequest(new { message = exception.Message })
        };
    }
}
