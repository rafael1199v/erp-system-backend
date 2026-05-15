using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Application.UseCases.Waiters;
using Erp.Sales.Presentation.ContractDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Sales.Presentation.Controllers.Contract;

[ApiController]
[Route("api/sales/companies/{companyCen}/waiters")]
public class WaitersContractController(
    ISalesCenResolver salesCenResolver,
    IGetWaiterOptionsByCompanyUseCase getWaiterOptionsByCompanyUseCase) : ControllerBase
{
    [EndpointSummary("Lista meseros por empresa")]
    [EndpointDescription("""
                         Devuelve las opciones de meseros disponibles para la empresa.
                         Usar para asignar meseros en tickets.
                         """)]
    [ProducesResponseType(typeof(List<WaiterContractResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [HttpGet]
    public async Task<IActionResult> GetWaiters(string companyCen)
    {
        int? companyId = await salesCenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada" });
        }

        List<WaiterOptionDto> waiters = await getWaiterOptionsByCompanyUseCase.ExecuteAsync(companyId.Value);
        return Ok(waiters.Select(waiter => new WaiterContractResponse
        {
            WaiterCen = waiter.WaiterCen,
            Name = waiter.Name
        }).ToList());
    }
}
