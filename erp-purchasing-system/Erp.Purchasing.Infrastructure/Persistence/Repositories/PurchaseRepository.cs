using Erp.Purchasing.Application.DTOs;
using Erp.Purchasing.Application.Repositories;
using Erp.Purchasing.Domain.Entities;
using Erp.Purchasing.Infrastructure.Persistence.Context;
using Erp.Purchasing.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Erp.Purchasing.Infrastructure.Persistence.Repositories;

public class PurchaseRepository(PurchasingDbContext context) : IPurchaseRepository
{
    public async Task AddAsync(Purchase purchase, CancellationToken ct = default)
    {
        await context.Purchases.AddAsync(ToModel(purchase), ct);
    }

    public async Task<Purchase?> GetEditableByCompanyAndCenAsync(
        string companyCen,
        string orderCen,
        CancellationToken ct = default)
    {
        if (!Guid.TryParse(companyCen, out var parsedCompanyCen)
            || !Guid.TryParse(orderCen, out var parsedOrderCen))
        {
            return null;
        }

        var model = await context.Purchases
            .AsNoTracking()
            .Include(purchase => purchase.PurchaseItems)
            .FirstOrDefaultAsync(purchase =>
                purchase.CompanyCen == parsedCompanyCen
                && purchase.Cen == parsedOrderCen,
                ct);

        return model is null ? null : ToDomain(model);
    }

    public async Task UpdateAsync(Purchase purchase, CancellationToken ct = default)
    {
        if (!Guid.TryParse(purchase.Cen, out var parsedPurchaseCen))
        {
            return;
        }

        var model = await context.Purchases
            .FirstOrDefaultAsync(candidate => candidate.Cen == parsedPurchaseCen, ct);

        if (model is null)
        {
            return;
        }

        model.PurchaseStatus = purchase.Status;
        model.ConfirmedAt = purchase.ConfirmedAt;
    }

    public async Task<PagedResultDto<PurchaseOrderListDto>> GetPagedAsync(
        string companyCen,
        PurchaseOrderQueryDto query,
        CancellationToken ct = default)
    {
        if (!Guid.TryParse(companyCen, out var parsedCompanyCen))
        {
            return new PagedResultDto<PurchaseOrderListDto>(
                Array.Empty<PurchaseOrderListDto>(),
                0,
                0,
                query.Page);
        }

        var baseQuery = context.Purchases
            .AsNoTracking()
            .Where(purchase => purchase.CompanyCen == parsedCompanyCen);

        if (query.Status.HasValue)
        {
            baseQuery = baseQuery.Where(purchase => purchase.PurchaseStatus == query.Status.Value);
        }

        var totalCount = await baseQuery.CountAsync(ct);
        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(totalCount / (double)query.PageSize);

        var orderedQuery = query.SortDescending
            ? baseQuery.OrderByDescending(purchase => purchase.CreatedAt)
            : baseQuery.OrderBy(purchase => purchase.CreatedAt);

        var items = await orderedQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(purchase => new PurchaseOrderListDto(
                purchase.Cen.ToString(),
                purchase.PurchaseStatus,
                purchase.CreatedAt,
                purchase.ConfirmedAt,
                purchase.Supplier.Cen.ToString(),
                purchase.PurchaseItems.Count))
            .ToListAsync(ct);

        return new PagedResultDto<PurchaseOrderListDto>(
            items,
            totalCount,
            totalPages,
            query.Page);
    }

    public async Task<PurchaseOrderDetailDto?> GetDetailAsync(
        string companyCen,
        string orderCen,
        CancellationToken ct = default)
    {
        if (!Guid.TryParse(companyCen, out var parsedCompanyCen)
            || !Guid.TryParse(orderCen, out var parsedOrderCen))
        {
            return null;
        }

        return await context.Purchases
            .AsNoTracking()
            .Where(purchase =>
                purchase.CompanyCen == parsedCompanyCen
                && purchase.Cen == parsedOrderCen)
            .Select(purchase => new PurchaseOrderDetailDto(
                purchase.Cen.ToString(),
                purchase.PurchaseStatus,
                purchase.CreatedAt,
                purchase.ConfirmedAt,
                purchase.Supplier.Cen.ToString(),
                purchase.WarehouseCen.ToString(),
                purchase.PurchaseItems
                    .Select(item => new PurchaseOrderDetailItemDto(
                        item.ProductCen.ToString(),
                        item.Quantity))
                    .ToList()))
            .FirstOrDefaultAsync(ct);
    }

    private static PurchaseModel ToModel(Purchase purchase)
    {
        return new PurchaseModel
        {
            Cen = Guid.Parse(purchase.Cen),
            CompanyCen = Guid.Parse(purchase.CompanyCen),
            SupplierId = purchase.SupplierId,
            PurchaseStatus = purchase.Status,
            CreatedAt = purchase.CreatedAt,
            ConfirmedAt = purchase.ConfirmedAt,
            WarehouseCen = Guid.Parse(purchase.WarehouseCen),
            PurchaseItems = purchase.Items.Select(item => new PurchaseItemModel
            {
                ProductCen = Guid.Parse(item.ProductCen),
                Quantity = item.Quantity,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }).ToList(),
            IsActive = true
        };
    }

    private static Purchase ToDomain(PurchaseModel model)
    {
        return Purchase.Restore(
            model.Id,
            model.Cen.ToString(),
            model.CompanyCen.ToString(),
            model.SupplierId,
            model.PurchaseStatus,
            model.CreatedAt,
            model.ConfirmedAt,
            model.PurchaseItems.Select(item => PurchaseItem.Restore(
                item.Id,
                item.ProductCen.ToString(),
                item.Quantity,
                item.PurchaseId)),
            model.WarehouseCen.ToString());
    }
}
