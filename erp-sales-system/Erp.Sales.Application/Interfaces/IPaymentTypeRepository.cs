using Erp.Sales.Domain.Entities;

namespace Erp.Sales.Application.Interfaces;

public interface IPaymentTypeRepository
{
    Task<bool> ExistsAsync(int paymentTypeId);
    Task<List<PaymentTypeOption>> GetAllAsync();
}
