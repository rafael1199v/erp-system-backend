using Erp.Sales.Domain.Entities;

namespace Erp.Sales.Application.Interfaces;

public interface IPaymentProcessRepository
{
    Task<SalesCenLookup> CreateSaleAndCloseOrderAsync(int restaurantOrderId, Sale sale);
}
