using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Product;

public interface IDeactivateProductUseCase
{
    Task ExecuteAsync(DeactivateProductDto productDto);
}

