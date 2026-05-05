namespace Erp.Sales.Application.Interfaces;

public interface IOrderRepository
{
    Task<int?> GetCompanyId(int orderId);
}
