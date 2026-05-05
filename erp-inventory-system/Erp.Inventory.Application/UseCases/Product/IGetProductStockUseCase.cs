using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Product;

public interface IGetProductStockUseCase
{
    Task<List<GetProductStockDTO>> ExecuteAsync(int companyId);
}