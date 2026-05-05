using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;

namespace Erp.Inventory.Application.UseCases.Product;

public class DeactivateProductUseCase(IProductRepository productRepository) : IDeactivateProductUseCase
{
    public async Task ExecuteAsync(DeactivateProductDto productDto)
    {
        var wasDeactivated = await productRepository.DeactivateAsync(productDto.ProductId, productDto.CompanyId);

        if (!wasDeactivated)
        {
            throw new Exception("No se encontro el producto para desactivar");
        }
    }
}

