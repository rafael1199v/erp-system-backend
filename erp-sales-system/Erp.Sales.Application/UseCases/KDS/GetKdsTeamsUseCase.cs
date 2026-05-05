using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;

namespace Erp.Sales.Application.UseCases.KDS;

public class GetKdsTeamsUseCase(IKdsRepository kdsRepository) : IGetKdsTeamsUseCase
{
    public async Task<List<KdsTeamDto>> ExecuteAsync(int companyId)
    {
        var teams = await kdsRepository.GetTeamsByCompanyIdAsync(companyId);

        return [.. teams
            .Select(t => new KdsTeamDto(
                Id: t.Id,
                Name: t.Name,
                CategoryIds: t.CategoryIds))];
    }
}
