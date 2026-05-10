using Erp.Purchasing.Application.DTOs;
using Erp.Purchasing.Application.Exceptions;
using Erp.Purchasing.Application.Repositories;
using Erp.Purchasing.Application.Services;
using Erp.Purchasing.Domain.Enums;

namespace Erp.Purchasing.Application.UseCases;

public class ConfirmPurchaseUseCase(
    IPurchaseRepository purchaseRepository,
    IPurchasingInventoryService inventoryService,
    IUnitOfWork unitOfWork) : IConfirmPurchaseUseCase
{
    public async Task<PurchaseOrderConfirmationDto> ExecuteAsync(
        string companyCen,
        string orderCen,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(companyCen))
        {
            throw new PurchasingBusinessException("El CEN de la empresa es requerido.");
        }

        if (string.IsNullOrWhiteSpace(orderCen))
        {
            throw new PurchasingBusinessException("El CEN de la orden es requerido.");
        }

        var purchase = await purchaseRepository.GetEditableByCompanyAndCenAsync(companyCen, orderCen, ct)
                       ?? throw new PurchasingNotFoundException("La orden de compra no existe.");

        if (purchase.Status != PurchaseStatus.Pending)
        {
            throw new PurchasingBusinessException("Solo se pueden confirmar ordenes de compra pendientes.");
        }

        var inventoryItems = purchase.Items
            .Select(item => new PurchaseOrderDetailItemDto(item.ProductCen, item.Quantity))
            .ToList();

        await inventoryService.ConfirmStockIncreaseAsync(companyCen, inventoryItems, ct);

        var confirmedAt = DateTime.UtcNow;
        purchase.Confirm(confirmedAt);
        await purchaseRepository.UpdateAsync(purchase, ct);
        await unitOfWork.CommitAsync(ct);

        return new PurchaseOrderConfirmationDto(purchase.Cen, purchase.Status, confirmedAt);
    }
}
