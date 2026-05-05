using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Product;

public interface ICreateOwnProductUseCase
{
    Task<int> ExecuteAsync(CreateProductDto productDto);
}