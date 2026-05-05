using Erp.Inventory.Application.DTOs;

namespace Erp.Inventory.Application.UseCases.Product;

public interface IActiveProductUseCase
{
    Task<bool> ExecuteAsync(ActivateProductDto activateProductDto);
}