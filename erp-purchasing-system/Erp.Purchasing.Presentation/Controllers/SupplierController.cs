using Erp.Purchasing.Application.DTOs;
using Erp.Purchasing.Application.Exceptions;
using Erp.Purchasing.Application.UseCases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Purchasing.Presentation.Controllers;

[ApiController]
[Route("api/purchases/companies/{companyCen}/suppliers")]
public class SupplierController(IGetSuppliersUseCase getSuppliersUseCase) : ControllerBase
{
    [EndpointSummary("Lista proveedores de una empresa")]
    [EndpointDescription("""
                         Devuelve los proveedores disponibles para compras.
                         Usar para poblar listas de seleccion en ordenes de compra.
                         """)]
    [ProducesResponseType(typeof(IReadOnlyCollection<SupplierDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [HttpGet]
    public async Task<IActionResult> GetSuppliers(string companyCen, CancellationToken ct = default)
    {
        try
        {
            return Ok(await getSuppliersUseCase.ExecuteAsync(companyCen, ct));
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
            PurchasingBusinessException => BadRequest(new { message = exception.Message }),
            InvalidOperationException => BadRequest(new { message = exception.Message }),
            _ => BadRequest(new { message = exception.Message })
        };
    }
}
