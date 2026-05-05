using Erp.Sales.Domain.Entities;

namespace Erp.Sales.Application.Interfaces;

public interface IWaiterRepository
{
    Task<List<Waiter>> GetWaitersByCompanyAsync(int companyId);
}