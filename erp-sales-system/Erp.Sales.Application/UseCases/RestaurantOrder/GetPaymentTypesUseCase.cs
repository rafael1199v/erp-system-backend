using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public class GetPaymentTypesUseCase(IPaymentTypeRepository paymentTypeRepository) : IGetPaymentTypesUseCase
{
    public async Task<List<PaymentTypeDto>> ExecuteAsync()
    {
        var paymentTypes = await paymentTypeRepository.GetAllAsync();

        return paymentTypes.Select(paymentType => new PaymentTypeDto
        {
            Id = paymentType.Id,
            Name = paymentType.Name
        }).ToList();
    }
}
