using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.UseCases.Category;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers;

[ApiController]
[Route("api/inventory/[controller]")]
public class CategoryController(ICreateCategoryUseCase createCategoryUseCase,
    IGetCategoriesByCompanyUseCase getCategoriesByCompanyUseCase,
    IUpdateCategoryUseCase updateCategoryUseCase) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
    {
        try
        {
            return Ok(await createCategoryUseCase.ExecuteAsync(createCategoryDto));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{companyId}")]
    public async Task<IActionResult> GetAllCategoriesByCompany(int companyId)
    {
        try
        {
            return Ok(await getCategoriesByCompanyUseCase.ExecuteAsync(companyId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCategory([FromBody] CategoryDto categoryDto)
    {
        try
        {
            await updateCategoryUseCase.ExecuteAsync(categoryDto);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
}