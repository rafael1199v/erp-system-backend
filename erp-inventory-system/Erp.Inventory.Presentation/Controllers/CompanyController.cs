using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.UseCases.Company;
using Erp.Inventory.Application.UseCases.Product;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers;

[Route("api/inventory/[controller]")]
[ApiController]
public class CompanyController : ControllerBase
{
    private readonly IGetCompaniesUseCase _getCompaniesUseCase;
    private readonly IGetProductStockUseCase _getProductStockUseCase;
    private readonly IGetProductCatalogUseCase _getProductCatalogUseCase;

    public CompanyController(
        IGetCompaniesUseCase getCompaniesUseCase,
        IGetProductStockUseCase getProductStockUseCase,
        IGetProductCatalogUseCase getProductCatalogUseCase)
    {
        this._getCompaniesUseCase = getCompaniesUseCase;
        this._getProductStockUseCase = getProductStockUseCase;
        this._getProductCatalogUseCase = getProductCatalogUseCase;
    }

    [HttpGet]
    public async Task<ActionResult<List<GetCompanyDTO>>> GetCompanies()
    {
        return Ok(await _getCompaniesUseCase.ExecuteAsync());
    }

    [HttpGet("{companyId}/products/stock")]
    public async Task<ActionResult<List<GetProductStockDTO>>> GetProductStock(int companyId)
    {
        return Ok(await _getProductStockUseCase.ExecuteAsync(companyId));
    }
    
    [HttpGet("{companyId}/products")]
    public async Task<ActionResult<List<GetProductCatalogDTO>>> GetProductCatalog(int companyId)
    {
        return Ok(await _getProductCatalogUseCase.ExecuteAsync(companyId));
    }
}