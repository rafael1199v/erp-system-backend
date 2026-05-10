using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.UseCases.Unit;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers;

[ApiController]
[Route("api/inventory/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class UnitController(
    ICreateUnitUseCase createUnitUseCase,
    IGetUnitsByCompanyUseCase getUnitsByCompanyUseCase,
    IUpdateUnitUseCase updateUnitUseCase) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateUnit(CreateUnitDto createUnitDto)
    {
        try
        {
            return Ok(await createUnitUseCase.ExecuteAsync(createUnitDto));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{companyId:int}")]
    public async Task<IActionResult> GetUnitsByCompany(int companyId)
    {
        try
        {
            return Ok(await getUnitsByCompanyUseCase.ExecuteAsync(companyId));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUnit(UnitDto unitDto)
    {
        try
        {
            await updateUnitUseCase.ExecuteAsync(unitDto);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}