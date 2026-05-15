using Erp.Inventory.Contracts;
using Erp.Inventory.Presentation.ContractAdapters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers.Contract;

[Route("api/inventory/companies/{companyCen}/sellable-products")]
public class InventorySellableProductsContractController(IInventoryProductContractAdapter productAdapter)
    : InventoryContractControllerBase
{
    [EndpointSummary("Lista productos vendibles")]
    [EndpointDescription("""
                         Devuelve productos disponibles para venta con filtros y paginacion.
                         Usar para catalogos comerciales o POS.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(List<SellableProductContractDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpGet]
    public async Task<IActionResult> GetSellableProducts(
        string companyCen,
        [FromQuery] string? search,
        [FromQuery] string? categoryCen,
        [FromQuery] string? warehouseCen,
        [FromQuery] bool onlyAvailable = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        return ToActionResult(await productAdapter.GetSellableProductsAsync(
            companyCen,
            search,
            categoryCen,
            warehouseCen,
            onlyAvailable,
            page,
            pageSize));
    }
}
