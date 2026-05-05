using Erp.Sales.Domain.Entities;
using Erp.Sales.Domain.Enums;

namespace Erp.Sales.Application.Interfaces;

public interface IKdsRepository
{
    Task<List<KdsTeam>> GetTeamsByCompanyIdAsync(int companyId);
    Task<KdsTeam?> GetTeamByIdAsync(int companyId, int teamId);
    Task<List<KdsOrderDetail>> GetOrderItemsByCompanyIdAsync(int companyId, DateTime fromUtc);
    Task<Dictionary<OrderDetailStatus, int>> CountOrderItemsByStatusAsync(int companyId, DateTime fromUtc, DateTime toUtc);
    Task UpdateKdsItemStatusAsync(int restaurantOrderDetailId, OrderDetailStatus newStatus);
}
