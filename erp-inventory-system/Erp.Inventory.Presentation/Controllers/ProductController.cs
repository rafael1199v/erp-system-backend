using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.UseCases.Product;
using Erp.Inventory.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers;


[Route("api/inventory/[controller]")]
[ApiController]
public class ProductController(IGetProductWithWarehousesUseCase getProductWithWarehousesUseCase,
    ICreateOwnProductUseCase createOwnProductUseCase,
    IUpdateOwnProductUseCase updateOwnProductUseCase,
    IDeactivateProductUseCase deactivateProductUseCase,
    IGetProductWithCompanyUseCase getProductWithCompanyUseCase,
    IActiveProductUseCase activeProductUseCase,
    IInventoryService inventoryService)
    : ControllerBase
{
    [HttpGet("stock/warehouses/{companyId}")]
    public async Task<IActionResult> GetProductsWithWarehouses(int companyId)
    {
        return Ok(await getProductWithWarehousesUseCase.ExecuteAsync(companyId));
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        try
        {
            return Ok(await createOwnProductUseCase.ExecuteAsync(createProductDto));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDto updateProductDto)
    {
        try
        {
            await updateOwnProductUseCase.ExecuteAsync(updateProductDto);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("deactivate")]
    public async Task<IActionResult> DeactivateProduct([FromBody] DeactivateProductDto deactivateProductDto)
    {
        try
        {
            await deactivateProductUseCase.ExecuteAsync(deactivateProductDto);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("activate")]
    public async Task<IActionResult> ActivateProduct([FromBody] ActivateProductDto activateProductDto)
    {
        try
        {
            await activeProductUseCase.ExecuteAsync(activateProductDto);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("with-company/{productId}")]
    public async Task<IActionResult> GetProductWithCompany(int productId)
    {
        try
        {
            return Ok(await getProductWithCompanyUseCase.GetProductWithCompanyAsync(productId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{productId:int}/is-active/{companyId:int}")]
    public async Task<IActionResult> CheckProductIsActive(int productId, int companyId)
    {
        try
        {
            return Ok(await inventoryService.IsProductActiveAsync(productId, companyId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{productId:int}/available-stock/{companyId:int}/{warehouseId:int}/{requestedQuantity:int}")]
    public async Task<IActionResult> CheckStockAvailability(int productId, int companyId, int warehouseId,
        int requestedQuantity)
    {
        try
        {
            return Ok(await inventoryService.HasAvailableStockAsync(productId, requestedQuantity, companyId,
                warehouseId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("kds/{companyId:int}/{warehouseId:int}")]
    public async Task<IActionResult> GetKdsProducts(int companyId, int warehouseId)
    {
        try
        {
            return Ok(await inventoryService.GetOrderDetailProductsAsync(companyId, warehouseId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("kds")]
    public async Task<IActionResult> GetKdsProductsByIds([FromBody] List<int> productIds)
    {
        try
        {
            return Ok(await inventoryService.GetOrderDetailProductsByIdsAsync(productIds));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }   
    }

    [HttpPost("valid-stock")]
    public async Task<IActionResult> ValidateProductStock([FromBody] StockValidationDto stockValidationDto)
    {
        try
        {
            return Ok(await inventoryService.ValidateStockAvailabilityAsync(stockValidationDto.Requirements,
                stockValidationDto.CompanyId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }  
    }
    
}