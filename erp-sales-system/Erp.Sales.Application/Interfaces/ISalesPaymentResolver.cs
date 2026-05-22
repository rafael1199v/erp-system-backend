namespace Erp.Sales.Application.Interfaces;

public interface ISalesPaymentResolver
{
    Task<int?> ResolvePaymentIdByCode(string paymentMethodCode);
}