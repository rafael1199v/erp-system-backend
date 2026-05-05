using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;

namespace Erp.Inventory.Application.UseCases.Product;

public class ActiveProductUseCase(IProductRepository productRepository) : IActiveProductUseCase
{
    public async Task<bool> ExecuteAsync(ActivateProductDto activateProductDto)
    {
        var wasActivated = await productRepository.ActiveAsync(activateProductDto.ProductId, activateProductDto.CompanyId);
        
        if (!wasActivated)
        {
            throw new Exception("No se encontro el producto para activarlo");
        }

        return true;
    }
}