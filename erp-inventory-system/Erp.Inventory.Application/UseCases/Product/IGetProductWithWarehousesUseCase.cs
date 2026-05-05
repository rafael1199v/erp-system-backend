using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Product;

public interface IGetProductWithWarehousesUseCase
{
    Task<List<ProductWarehouseDTO>> ExecuteAsync(int companyId);
}