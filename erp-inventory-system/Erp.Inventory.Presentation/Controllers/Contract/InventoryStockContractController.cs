using Erp.Inventory.Presentation.ContractAdapters;
using Erp.Inventory.Presentation.ContractDtos;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers.Contract;

[Route("api/inventory/companies/{companyCen}/stock")]
public class InventoryStockContractController(
    IInventoryStockContractAdapter stockAdapter,
    IInventoryMovementContractAdapter movementAdapter)
    : InventoryContractControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetStock(
        string companyCen,
        [FromQuery] string? productCen,
        [FromQuery] string? warehouseCen)
    {
        return ToActionResult(await stockAdapter.GetStockAsync(companyCen, productCen, warehouseCen));
    }

    [HttpPost("validate")]
    public async Task<IActionResult> ValidateStock(string companyCen, [FromBody] StockValidationContractRequest request)
    {
        return ToActionResult(await stockAdapter.ValidateStockAsync(companyCen, request));
    }

    [HttpPost("consume")]
    public async Task<IActionResult> ConsumeStock(string companyCen, [FromBody] StockConsumeContractRequest request)
    {
        return ToActionResult(await stockAdapter.ConsumeStockAsync(companyCen, request));
    }

    [HttpPost("adjustments")]
    public async Task<IActionResult> CreateAdjustment(string companyCen, [FromBody] InventoryAdjustmentContractRequest request)
    {
        return ToCreatedResult(await movementAdapter.CreateAdjustmentAsync(companyCen, request));
    }
}
