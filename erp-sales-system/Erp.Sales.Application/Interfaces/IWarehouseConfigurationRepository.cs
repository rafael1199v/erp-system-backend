namespace Erp.Sales.Application.Interfaces;

public interface IWarehouseConfigurationRepository
{
    Task<int?> GetWarehouseIdByCompanyIdAsync(int companyId);
    Task<string?> GetWarehouseCenByCompanyIdAsync(int companyId);
}
