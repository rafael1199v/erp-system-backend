using Erp.Sales.Application.DTOs;

namespace Erp.Sales.Application.UseCases.KDS;

public interface IGetKdsTeamsUseCase
{
    Task<List<KdsTeamDto>> ExecuteAsync(int companyId);
}
