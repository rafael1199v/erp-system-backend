namespace Erp.Sales.Application.Interfaces;

public interface IWarehouseConfigurationRepository
{
    Task<int?> GetWarehouseIdByCompanyIdAsync(int companyId);
}