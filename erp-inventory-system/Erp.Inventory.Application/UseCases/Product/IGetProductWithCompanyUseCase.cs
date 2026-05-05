using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Product;

public interface IGetProductWithCompanyUseCase
{
    Task<ProductDto> GetProductWithCompanyAsync(int productId);
}