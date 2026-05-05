using Erp.Sales.Application.DTOs;

namespace Erp.Sales.Application.UseCases.KDS;

public interface IGetKdsTeamItemsUseCase
{
    Task<List<KdsOrderItem>> ExecuteAsync(int companyId, int teamId);
}
