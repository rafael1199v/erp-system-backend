using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.UseCases.Movement;
using Erp.Inventory.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers;

[Route("api/inventory/[controller]")]
[ApiController]
public class MovementController : ControllerBase
{
    private readonly ICreateAdjustmentMovementUseCase _createAdjustmentMovementUseCase;
    private readonly IGetMovementsUseCase _getMovementsUseCase;
    private readonly ICreateMovementUseCase _createMovementUseCase;
    private readonly IInventoryService _inventoryService;

    public MovementController(ICreateAdjustmentMovementUseCase createAdjustmentMovementUseCase, IGetMovementsUseCase getMovementsUseCase,
    ICreateMovementUseCase createMovementUseCase, IInventoryService inventoryService)
    {
        this._createAdjustmentMovementUseCase = createAdjustmentMovementUseCase;
        this._getMovementsUseCase = getMovementsUseCase;
        this._createMovementUseCase = createMovementUseCase;
        this._inventoryService = inventoryService;
    }
    
    [HttpPost("adjustment")]
    public async Task<IActionResult> CreateAdjustmentMovement([FromBody] CreateInventoryMovementDTO createInventoryMovementDto)
    {
        try
        {
            await this._createAdjustmentMovementUseCase.ExecuteAsync(createInventoryMovementDto);
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message});
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateMovement([FromBody] CreateInventoryMovementDTO createInventoryDto)
    {
        try
        {
            await _createMovementUseCase.ExecuteAsync(createInventoryDto);
            return Created();            
        }
        catch(Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{companyId}")]
    public async Task<IActionResult> GetMovements(int companyId, [FromQuery] int? movementType)
    {
        try
        {
            var result = movementType.HasValue
                ? await _getMovementsUseCase.GetMovementsByTypeAsync(movementType.Value, companyId)
                : await _getMovementsUseCase.GetMovementsAsync(companyId);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { messag = ex.Message });
        }
    }
    
    [HttpPost("payment")]
    public async Task<IActionResult> CreatePaymentMovement([FromBody] CreatePaymentStockDiscountDto createInventoryMovementDto)
    {
        try
        {
            await _inventoryService.ExecutePaymentStockDiscountAsync(createInventoryMovementDto);
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
}