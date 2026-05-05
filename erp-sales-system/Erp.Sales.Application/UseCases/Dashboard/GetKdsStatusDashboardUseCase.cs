using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Domain.Enums;

namespace Erp.Sales.Application.UseCases.Dashboard;

public class GetKdsStatusDashboardUseCase(IKdsRepository kdsRepository) : IGetKdsStatusDashboardUseCase
{
    public async Task<KdsStatusDashboardDto> ExecuteAsync(int companyId)
    {
        var (startUtc, endUtc) = BoliviaDateRangeHelper.GetCurrentDayUtcRange();

        Dictionary<OrderDetailStatus, int> countsByStatus =
            await kdsRepository.CountOrderItemsByStatusAsync(companyId, startUtc, endUtc);

        return new KdsStatusDashboardDto(
            PendingCount: countsByStatus.GetValueOrDefault(OrderDetailStatus.Created, 0),
            PreparingCount: countsByStatus.GetValueOrDefault(OrderDetailStatus.Preparing, 0),
            ReadyCount: countsByStatus.GetValueOrDefault(OrderDetailStatus.Delivered, 0));
    }
}