using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Product;

public interface IUpdateOwnProductUseCase
{
    Task ExecuteAsync(UpdateProductDto productDto);
}

