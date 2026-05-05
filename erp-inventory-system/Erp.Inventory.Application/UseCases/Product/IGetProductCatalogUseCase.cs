using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Product;

public interface IGetProductCatalogUseCase
{
    Task<List<GetProductCatalogDTO>> ExecuteAsync(int companyId);
}