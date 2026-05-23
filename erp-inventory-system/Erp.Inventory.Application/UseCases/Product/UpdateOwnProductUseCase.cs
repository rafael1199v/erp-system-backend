using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.UseCases.Product;

public class UpdateOwnProductUseCase(IProductRepository productRepository) : IUpdateOwnProductUseCase
{
    public async Task ExecuteAsync(UpdateProductDto productDto)
    {
        var productCompanyEntity = ProductCompanyEntity.UpdateOwnProduct(
            productId: productDto.ProductId,
            name: productDto.Name,
            imageUrl: productDto.ImageUrl,
            unitId: productDto.UnitId,
            companyId: productDto.CompanyId,
            categoryId: productDto.CategoryId,
            productStatusId: productDto.ProductStatusId,
            supplierId: productDto.SupplierId,
            currentCost: productDto.CurrentCost,
            reorderLevel: productDto.ReorderLevel,
            sellPrice: productDto.SellPrice,
            sku: productDto.Sku,
            description: productDto.Description,
            stationCode: productDto.StationCode
        );

        var wasUpdated = await productRepository.UpdateOwnProductAsync(productCompanyEntity);

        if (!wasUpdated)
        {
            throw new Exception("No se encontro el producto para actualizar");
        }
    }
}

