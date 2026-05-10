using Erp.Inventory.Application.UseCases.Transaction;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers;

[ApiController]
[Route("api/inventory/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class TransactionController : ControllerBase
{
    private readonly IGetTransactionDetailsUseCase _getTransactionDetailsUseCase;

    public TransactionController(IGetTransactionDetailsUseCase getTransactionDetailsUseCase)
    {
        this._getTransactionDetailsUseCase = getTransactionDetailsUseCase;
    }
    
    [HttpGet("details/{productId}")]
    public async Task<IActionResult> GetTransactionDetails(int productId)
    {
        try
        {
            return Ok(await _getTransactionDetailsUseCase.ExecuteAsync(productId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}