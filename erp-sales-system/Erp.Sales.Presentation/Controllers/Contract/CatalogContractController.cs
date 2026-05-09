using Erp.Inventory.Contracts;
using Erp.Sales.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Sales.Presentation.Controllers.Contract;

[ApiController]
[Route("api/sales/companies/{companyCen}/catalog")]
public class CatalogContractController(
    ISalesCenResolver salesCenResolver,
    IWarehouseConfigurationRepository warehouseConfigurationRepository,
    IInventoryService inventoryService)
    : ControllerBase
{
    [HttpGet("products")]
    public async Task<IActionResult> GetProducts(
        string companyCen,
        [FromQuery] string? search,
        [FromQuery] string? categoryCen,
        [FromQuery] string? warehouseCen,
        [FromQuery] bool onlyAvailable = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        int? companyId = await salesCenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada" });
        }

        string? effectiveWarehouseCen = string.IsNullOrWhiteSpace(warehouseCen)
            ? await warehouseConfigurationRepository.GetWarehouseCenByCompanyIdAsync(companyId.Value)
            : warehouseCen.Trim();

        if (string.IsNullOrWhiteSpace(effectiveWarehouseCen))
        {
            return BadRequest(new { message = "No hay una bodega principal configurada para la compania" });
        }

        List<SellableProductContractDto> products = await inventoryService.GetSellableProductsAsync(
            companyCen,
            search,
            categoryCen,
            effectiveWarehouseCen,
            onlyAvailable,
            page,
            pageSize);

        return Ok(products);
    }
}
