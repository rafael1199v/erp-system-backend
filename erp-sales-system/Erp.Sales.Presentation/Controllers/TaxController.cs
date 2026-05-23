using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.UseCases.TaxConfiguration;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Sales.Presentation.Controllers;

[ApiController]
[Route("api/sales/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class TaxController(IUpdateGlobalTaxUseCase updateGlobalTaxUseCase,
    ICreateGlobalTaxUseCase createGlobalTaxUseCase,
    IGetGlobalTaxUseCase getGlobalTaxUseCase) : ControllerBase
{

    [HttpGet("{companyId}")]
    public async Task<IActionResult> GetGlobalTax(int companyId)
    {
        try
        {
            return Ok(await getGlobalTaxUseCase.ExecuteAsync(companyId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateGlobalTax([FromBody] CreateGlobalTaxDto createGlobalTaxDto)
    {
        try
        {
            await createGlobalTaxUseCase.ExecuteAsync(createGlobalTaxDto);
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateGlobalTax([FromBody] UpdateGlobalTaxDto updateGlobalTaxDto)
    {
        try
        {
            await updateGlobalTaxUseCase.ExecuteAsync(updateGlobalTaxDto);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}