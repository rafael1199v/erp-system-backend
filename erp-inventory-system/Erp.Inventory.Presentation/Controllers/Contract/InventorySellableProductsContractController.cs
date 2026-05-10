using Erp.Inventory.Presentation.ContractAdapters;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers.Contract;

[Route("api/inventory/companies/{companyCen}/sellable-products")]
public class InventorySellableProductsContractController(IInventoryProductContractAdapter productAdapter)
    : InventoryContractControllerBase
{
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
