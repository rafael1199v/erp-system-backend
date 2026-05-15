using Erp.Inventory.Contracts;
using Erp.Inventory.Presentation.ContractAdapters;
using Erp.Inventory.Presentation.ContractDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers.Contract;

[Route("api/inventory/companies/{companyCen}")]
public class InventoryCatalogContractController(
    IInventoryCatalogContractAdapter catalogAdapter,
    IInventoryProductContractAdapter productAdapter,
    IInventoryStockContractAdapter stockAdapter)
    : InventoryContractControllerBase
{
    [EndpointSummary("Resume indicadores de catalogo y stock")]
    [EndpointDescription("""
                         Devuelve conteos agregados de productos y existencias para una empresa.
                         Usar para dashboards del modulo de inventario y reportes rapidos.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(InventoryDashboardContractDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
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

    [EndpointSummary("Lista categorias de productos")]
    [EndpointDescription("""
                         Devuelve las categorias registradas en inventario para la empresa.
                         Usar para filtros y formularios de productos.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(List<CategoryContractDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(string companyCen)
    {
        return ToActionResult(await catalogAdapter.GetCategoriesAsync(companyCen));
    }

    [EndpointSummary("Crea una categoria de productos")]
    [EndpointDescription("""
                         Registra una nueva categoria para organizar el catalogo.
                         Usar en administracion de inventario antes de crear productos.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(CategoryContractDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory(string companyCen, [FromBody] CreateCategoryContractRequest request)
    {
        return ToCreatedResult(await catalogAdapter.CreateCategoryAsync(companyCen, request));
    }

    [EndpointSummary("Actualiza una categoria de productos")]
    [EndpointDescription("""
                         Modifica los datos de una categoria existente.
                         Usar para renombrar o ajustar la configuracion de la categoria.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(CategoryContractDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpPut("categories/{categoryCen}")]
    public async Task<IActionResult> UpdateCategory(
        string companyCen,
        string categoryCen,
        [FromBody] CreateCategoryContractRequest request)
    {
        return ToActionResult(await catalogAdapter.UpdateCategoryAsync(companyCen, categoryCen, request));
    }

    [EndpointSummary("Lista unidades de medida")]
    [EndpointDescription("""
                         Devuelve las unidades de medida configuradas para la empresa.
                         Usar para formularios de productos y conversiones.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(List<UnitContractDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpGet("units")]
    public async Task<IActionResult> GetUnits(string companyCen)
    {
        return ToActionResult(await catalogAdapter.GetUnitsAsync(companyCen));
    }

    [EndpointSummary("Crea una unidad de medida")]
    [EndpointDescription("""
                         Registra una unidad de medida para productos e inventario.
                         Usar antes de crear productos con nuevas unidades.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(UnitContractDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpPost("units")]
    public async Task<IActionResult> CreateUnit(string companyCen, [FromBody] CreateUnitContractRequest request)
    {
        return ToCreatedResult(await catalogAdapter.CreateUnitAsync(companyCen, request));
    }

    [EndpointSummary("Actualiza una unidad de medida")]
    [EndpointDescription("""
                         Modifica los datos de una unidad existente.
                         Usar para mantener consistencia en productos y movimientos.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(UnitContractDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpPut("units/{unitCen}")]
    public async Task<IActionResult> UpdateUnit(
        string companyCen,
        string unitCen,
        [FromBody] CreateUnitContractRequest request)
    {
        return ToActionResult(await catalogAdapter.UpdateUnitAsync(companyCen, unitCen, request));
    }

    [EndpointSummary("Lista bodegas")]
    [EndpointDescription("""
                         Devuelve las bodegas configuradas para la empresa.
                         Usar para seleccionar origen o destino de movimientos de inventario.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(List<WarehouseContractDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpGet("warehouses")]
    public async Task<IActionResult> GetWarehouses(string companyCen)
    {
        return ToActionResult(await catalogAdapter.GetWarehousesAsync(companyCen));
    }
}
