using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Application.UseCases.Waiters;
using Erp.Sales.Presentation.ContractDtos;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Sales.Presentation.Controllers.Contract;

[ApiController]
[Route("api/sales/companies/{companyCen}/waiters")]
public class WaitersContractController(
    ISalesCenResolver salesCenResolver,
    IGetWaiterOptionsByCompanyUseCase getWaiterOptionsByCompanyUseCase) : ControllerBase
{
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
