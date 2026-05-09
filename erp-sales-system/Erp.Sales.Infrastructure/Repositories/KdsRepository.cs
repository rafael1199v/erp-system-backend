using Erp.Sales.Application.Interfaces;
using Erp.Sales.Domain.Entities;
using Erp.Sales.Domain.Enums;
using Erp.Sales.Infrastructure.Models.PoS;
using Microsoft.EntityFrameworkCore;

namespace Erp.Sales.Infrastructure.Repositories;

public class KdsRepository(SalesDbContext salesDbContext) : IKdsRepository
{
    public async Task<List<KdsTeam>> GetTeamsByCompanyIdAsync(int companyId)
    {
        var teamModels = await Queryable
            .Where<TeamModel>(salesDbContext.Teams
                .AsNoTracking(), t => t.CompanyId == companyId && !t.IsDeleted)
            .ToListAsync();

        var categoryRows = await Queryable
            .Where<TeamConfigurationModel>(salesDbContext.TeamConfigurations
                .AsNoTracking(), tc => tc.CompanyId == companyId)
            .ToListAsync();

        var categoryMap = Enumerable
            .GroupBy<TeamConfigurationModel, int>(categoryRows, tc => tc.TeamId)
            .ToDictionary(g => g.Key, g => g.Select(tc => tc.CategoryId).Distinct().ToList());

        return [.. teamModels
            .Select(t => new KdsTeam
            {
                Id = t.Id,
                Cen = t.Cen,
                Name = t.Name,
                CategoryIds = categoryMap.GetValueOrDefault(t.Id, []),
                CategoryCens = []
            })];
    }

    public async Task<KdsTeam?> GetTeamByIdAsync(int companyId, int teamId)
    {
        var teamModel = await salesDbContext.Teams
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.CompanyId == companyId && t.Id == teamId && !t.IsDeleted);

        if (teamModel is null)
        {
            return null;
        }

        var categoryIds = await Queryable
            .Where<TeamConfigurationModel>(salesDbContext.TeamConfigurations
                .AsNoTracking(), tc => tc.CompanyId == companyId && tc.TeamId == teamId)
            .Select(tc => tc.CategoryId)
            .Distinct()
            .ToListAsync();

        return new KdsTeam
        {
            Id = teamModel.Id,
            Cen = teamModel.Cen,
            Name = teamModel.Name,
            CategoryIds = categoryIds
        };
    }

    public async Task<List<KdsOrderDetail>> GetOrderItemsByCompanyIdAsync(int companyId, DateTime fromUtc)
    {
        return await salesDbContext.RestaurantOrderDetails
            .AsNoTracking()
            .Where(rod =>
                !rod.IsDeleted &&
                rod.CreatedAt >= fromUtc &&
                !rod.RestaurantOrder.IsDeleted &&
                rod.SentAt != null &&
                !rod.RestaurantOrder.Order.IsDeleted &&
                rod.RestaurantOrder.Order.CompanyId == companyId &&
                rod.RestaurantOrder.Order.OrderStatusId != (int)OrderStatus.Cancelled)
            .Select(rod => new KdsOrderDetail
            {
                ProductId = rod.ProductId,
                ProductCen = rod.ProductCen,
                RestaurantOrderDetailId = rod.Id,
                TicketItemCen = rod.Cen,
                RestaurantOrderId = rod.RestaurantOrderId,
                TicketCen = rod.RestaurantOrder.Cen,
                Quantity = rod.Quantity,
                RestaurantOrderDetailStatus = (OrderDetailStatus)rod.RestaurantOrderDetailStatusId,
                Note = rod.Note,
                CreatedAt = rod.CreatedAt,
                ResendCount = rod.ResendCount,
            })
            .ToListAsync();
    }

    public async Task<Dictionary<OrderDetailStatus, int>> CountOrderItemsByStatusAsync(int companyId, DateTime fromUtc,
        DateTime toUtc)
    {
        var statusIdsToInclude = new[]
        {
            (int)OrderDetailStatus.Created,
            (int)OrderDetailStatus.Preparing,
            (int)OrderDetailStatus.Delivered
        };

        var groupedCounts = await salesDbContext.RestaurantOrderDetails
            .AsNoTracking()
            .Where(rod =>
                !rod.IsDeleted &&
                rod.CreatedAt >= fromUtc &&
                rod.CreatedAt < toUtc &&
                rod.SentAt != null &&
                !rod.RestaurantOrder.IsDeleted &&
                !rod.RestaurantOrder.Order.IsDeleted &&
                rod.RestaurantOrder.Order.CompanyId == companyId &&
                rod.RestaurantOrder.Order.OrderStatusId != (int)OrderStatus.Cancelled &&
                statusIdsToInclude.Contains(rod.RestaurantOrderDetailStatusId))
            .GroupBy(rod => rod.RestaurantOrderDetailStatusId)
            .Select(g => new
            {
                StatusId = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        return groupedCounts.ToDictionary(
            keySelector: row => (OrderDetailStatus)row.StatusId,
            elementSelector: row => row.Count);
    }

    public async Task UpdateKdsItemStatusAsync(int restaurantOrderDetailId, OrderDetailStatus newStatus)
    {
        await Queryable
            .Where<RestaurantOrderDetailModel>(salesDbContext.RestaurantOrderDetails, rod => rod.Id == restaurantOrderDetailId && !rod.IsDeleted)
            .ExecuteUpdateAsync(rod => rod.SetProperty(r => r.RestaurantOrderDetailStatusId, (int)newStatus));
    }
}
