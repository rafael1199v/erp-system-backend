using Erp.Inventory.Contracts;
using Erp.Inventory.Presentation.ContractAdapters;
using Erp.Inventory.Presentation.ContractDtos;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers.Contract;

[Route("api/inventory/companies/{companyCen}/products")]
public class InventoryProductsContractController(IInventoryProductContractAdapter productAdapter)
    : InventoryContractControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        string companyCen,
        [FromQuery] string? search,
        [FromQuery] string? categoryCen,
        [FromQuery] string? status)
    {
        return ToActionResult(await productAdapter.GetProductsAsync(companyCen, search, categoryCen, status));
    }

    [HttpPost("lookup")]
    public async Task<IActionResult> LookupProducts(
        string companyCen,
        [FromBody] ProductLookupContractRequest request)
    {
        return ToActionResult(await productAdapter.LookupProductsAsync(companyCen, request));
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(string companyCen, [FromBody] CreateProductContractRequest request)
    {
        return ToCreatedResult(await productAdapter.CreateProductAsync(companyCen, request));
    }

    [HttpPut("{productCen}")]
    public async Task<IActionResult> UpdateProduct(
        string companyCen,
        string productCen,
        [FromBody] UpdateProductContractRequest request)
    {
        return ToActionResult(await productAdapter.UpdateProductAsync(companyCen, productCen, request));
    }

    [HttpPatch("{productCen}/status")]
    public async Task<IActionResult> UpdateProductStatus(
        string companyCen,
        string productCen,
        [FromBody] UpdateProductStatusContractRequest request)
    {
        return ToActionResult(await productAdapter.UpdateProductStatusAsync(companyCen, productCen, request));
    }
}
