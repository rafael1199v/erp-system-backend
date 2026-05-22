using Erp.Sales.Application.Interfaces;

namespace Erp.Sales.Infrastructure.Services;

public class SalesPaymentResolver(IPaymentTypeRepository paymentTypeRepository) : ISalesPaymentResolver
{
    public async Task<int?> ResolvePaymentIdByCode(string paymentMethodCode)
    {
        return await paymentTypeRepository.ResolveIdByCodeAsync(paymentMethodCode);
    }
}