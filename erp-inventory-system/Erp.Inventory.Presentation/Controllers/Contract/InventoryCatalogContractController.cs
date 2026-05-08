using Erp.Inventory.Presentation.ContractAdapters;
using Erp.Inventory.Presentation.ContractDtos;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers.Contract;

[Route("api/inventory/companies/{companyCen}")]
public class InventoryCatalogContractController(
    IInventoryCatalogContractAdapter catalogAdapter,
    IInventoryProductContractAdapter productAdapter,
    IInventoryStockContractAdapter stockAdapter)
    : InventoryContractControllerBase
{
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(string companyCen)
    {
        var productsResult = await productAdapter.GetProductsAsync(companyCen);
        if (!productsResult.Succeeded || productsResult.Value is null)
        {
            return ToActionResult(productsResult);
        }

        var stockResult = await stockAdapter.GetStockAsync(companyCen);
        if (!stockResult.Succeeded || stockResult.Value is null)
        {
            return ToActionResult(stockResult);
        }

        List<ProductContractDto> products = productsResult.Value;
        List<StockItemContractDto> stockItems = stockResult.Value;

        return Ok(new InventoryDashboardContractDto
        {
            CompanyCen = companyCen,
            TotalProducts = products.Count,
            TotalStockQuantity = stockItems.Sum(item => item.AvailableQuantity),
            LowStockCount = stockItems
                .Where(item => item.IsLowStock && item.AvailableQuantity > 0)
                .Select(item => item.ProductCen)
                .Distinct()
                .Count(),
            OutOfStockCount = stockItems
                .Where(item => item.AvailableQuantity == 0)
                .Select(item => item.ProductCen)
                .Distinct()
                .Count()
        });
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(string companyCen)
    {
        return ToActionResult(await catalogAdapter.GetCategoriesAsync(companyCen));
    }

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory(string companyCen, [FromBody] CreateCategoryContractRequest request)
    {
        return ToCreatedResult(await catalogAdapter.CreateCategoryAsync(companyCen, request));
    }

    [HttpPut("categories/{categoryCen}")]
    public async Task<IActionResult> UpdateCategory(
        string companyCen,
        string categoryCen,
        [FromBody] CreateCategoryContractRequest request)
    {
        return ToActionResult(await catalogAdapter.UpdateCategoryAsync(companyCen, categoryCen, request));
    }

    [HttpGet("units")]
    public async Task<IActionResult> GetUnits(string companyCen)
    {
        return ToActionResult(await catalogAdapter.GetUnitsAsync(companyCen));
    }

    [HttpPost("units")]
    public async Task<IActionResult> CreateUnit(string companyCen, [FromBody] CreateUnitContractRequest request)
    {
        return ToCreatedResult(await catalogAdapter.CreateUnitAsync(companyCen, request));
    }

    [HttpPut("units/{unitCen}")]
    public async Task<IActionResult> UpdateUnit(
        string companyCen,
        string unitCen,
        [FromBody] CreateUnitContractRequest request)
    {
        return ToActionResult(await catalogAdapter.UpdateUnitAsync(companyCen, unitCen, request));
    }

    [HttpGet("warehouses")]
    public async Task<IActionResult> GetWarehouses(string companyCen)
    {
        return ToActionResult(await catalogAdapter.GetWarehousesAsync(companyCen));
    }
}
