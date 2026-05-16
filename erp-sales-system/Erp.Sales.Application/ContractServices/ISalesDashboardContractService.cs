using Erp.Sales.Application.ContractDtos;

namespace Erp.Sales.Application.ContractServices;

public interface ISalesDashboardContractService
{
    Task<List<TopProductDashboardContractResponse>> GetTopProductsAsync(
        int companyId,
        string companyCen,
        int topN);
}
