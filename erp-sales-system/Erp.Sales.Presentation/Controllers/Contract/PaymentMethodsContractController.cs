using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Services;
using Erp.Sales.Application.UseCases.RestaurantOrder;
using Erp.Sales.Presentation.ContractDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Sales.Presentation.Controllers.Contract;

[ApiController]
[Route("api/sales/payment-methods")]
public class PaymentMethodsContractController(IGetPaymentTypesUseCase getPaymentTypesUseCase) : ControllerBase
{
    [EndpointSummary("Lista metodos de pago")]
    [EndpointDescription("""
                         Devuelve los metodos de pago disponibles para ventas.
                         Usar para opciones de pago al procesar tickets.
                         """)]
    [ProducesResponseType(typeof(List<PaymentMethodContractResponse>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetPaymentMethods()
    {
        List<PaymentTypeDto> paymentTypes = await getPaymentTypesUseCase.ExecuteAsync();

        return Ok(paymentTypes.Select(paymentType => new PaymentMethodContractResponse
        {
            PaymentMethodCode = PaymentMethodCodeNormalizer.Normalize(paymentType.Name),
            Name = paymentType.Name,
            IsActive = true
        }).ToList());
    }
}
