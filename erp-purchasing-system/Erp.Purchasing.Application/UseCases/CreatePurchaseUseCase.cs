using Erp.Purchasing.Application.DTOs;
using Erp.Purchasing.Application.Exceptions;
using Erp.Purchasing.Application.Repositories;
using Erp.Purchasing.Domain.Entities;

namespace Erp.Purchasing.Application.UseCases;

public class CreatePurchaseUseCase(
    ISupplierRepository supplierRepository,
    IPurchaseRepository purchaseRepository,
    IUnitOfWork unitOfWork) : ICreatePurchaseUseCase
{
    public async Task<PurchaseOrderSummaryDto> ExecuteAsync(
        string companyCen,
        CreatePurchaseOrderDto request,
        CancellationToken ct = default)
    {
        Validate(companyCen, request);

        var supplier = await supplierRepository.GetByCompanyAndCenAsync(companyCen, request.SupplierCen, ct)
                       ?? throw new PurchasingNotFoundException("El proveedor no existe para la empresa indicada.");

        var items = request.Items
            .Select(item => PurchaseItem.Create(item.ProductCen, item.Quantity))
            .ToList();

        var purchase = Purchase.Create(companyCen, supplier.Id, items, request.WarehouseCen);

        await purchaseRepository.AddAsync(purchase, ct);
        await unitOfWork.CommitAsync(ct);

        return new PurchaseOrderSummaryDto(purchase.Cen, purchase.Status);
    }

    private static void Validate(string companyCen, CreatePurchaseOrderDto request)
    {
        if (string.IsNullOrWhiteSpace(companyCen))
        {
            throw new PurchasingBusinessException("El CEN de la empresa es requerido.");
        }

        if (request is null)
        {
            throw new PurchasingBusinessException("La solicitud de compra es requerida.");
        }

        if (string.IsNullOrWhiteSpace(request.WarehouseCen))
        {
            throw new PurchasingBusinessException("El CEN del warehouse es requerido");
        }

        if (string.IsNullOrWhiteSpace(request.SupplierCen))
        {
            throw new PurchasingBusinessException("El CEN del proveedor es requerido.");
        }

        if (request.Items is null || request.Items.Count == 0)
        {
            throw new PurchasingBusinessException("La orden de compra debe tener al menos un item.");
        }

        foreach (var item in request.Items)
        {
            if (string.IsNullOrWhiteSpace(item.ProductCen))
            {
                throw new PurchasingBusinessException("El CEN del producto es requerido.");
            }

            if (item.Quantity <= 0)
            {
                throw new PurchasingBusinessException("La cantidad del item debe ser mayor a cero.");
            }
        }
    }
}
