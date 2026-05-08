using Erp.Inventory.Presentation.ContractAdapters;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers.Contract;

[Route("api/inventory/companies")]
public class InventoryCompaniesContractController(IInventoryCatalogContractAdapter catalogAdapter)
    : InventoryContractControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCompanies()
    {
        return Ok(await catalogAdapter.GetCompaniesAsync());
    }
}
