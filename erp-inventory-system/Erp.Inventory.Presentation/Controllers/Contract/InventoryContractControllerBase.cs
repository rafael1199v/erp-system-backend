using Erp.Inventory.Presentation.ContractAdapters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers.Contract;

[ApiController]
public abstract class InventoryContractControllerBase : ControllerBase
{
    protected IActionResult ToActionResult<T>(InventoryContractResult<T> result)
    {
        return result.Status switch
        {
            InventoryContractResultStatus.Success => Ok(result.Value),
            InventoryContractResultStatus.NotFound => NotFound(new { message = result.Message }),
            InventoryContractResultStatus.Conflict => Conflict(new { message = result.Message, data = result.Value }),
            InventoryContractResultStatus.Invalid => BadRequest(new { message = result.Message }),
            _ => BadRequest(new { message = result.Message ?? "Resultado de contrato no valido" })
        };
    }

    protected IActionResult ToCreatedResult<T>(InventoryContractResult<T> result)
    {
        return result.Status == InventoryContractResultStatus.Success
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : ToActionResult(result);
    }
}
