using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.UseCases.KDS;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Sales.Presentation.Controllers;

[ApiController]
[Route("api/sales/kds")]
public class KdsController(
    IGetKdsTeamsUseCase getKdsTeamsUseCase,
    IGetKdsTeamItemsUseCase getKdsTeamItemsUseCase,
    IChangeKdsItemStatusUseCase changeKdsItemStatusUseCase) : ControllerBase
{
    [HttpGet("{companyId:int}/teams")]
    public async Task<IActionResult> GetTeams(int companyId)
    {
        try
        {
            return Ok(await getKdsTeamsUseCase.ExecuteAsync(companyId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{companyId:int}/teams/{teamId:int}/items")]
    public async Task<IActionResult> GetTeamItems(int companyId, int teamId)
    {
        try
        {
            return Ok(await getKdsTeamItemsUseCase.ExecuteAsync(companyId, teamId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("restaurant-order-detail")]
    public async Task<IActionResult> UpdateKdsItemStatus([FromBody] UpdateKdsItemDto updateKdsItemDto)
    {
        try
        {
            await changeKdsItemStatusUseCase.ExecuteAsync(updateKdsItemDto.RestaurantOrderDetailId, updateKdsItemDto.NewStatusId);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
