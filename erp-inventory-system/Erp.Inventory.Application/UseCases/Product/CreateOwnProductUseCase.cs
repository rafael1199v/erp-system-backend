using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.UseCases.Product;

public class CreateOwnProductUseCase(IProductRepository productRepository) : ICreateOwnProductUseCase
{
    public async Task<int> ExecuteAsync(CreateProductDto productDto)
    {
        var productCompanyEntity = ProductCompanyEntity.CreateOwnProduct(
            name: productDto.Name,
            imageUrl: productDto.ImageUrl,
            unitId: productDto.UnitId,
            companyId: productDto.CompanyId,
            categoryId: productDto.CategoryId,
            productStatusId: productDto.ProductStatusId,
            supplierId: productDto.SupplierId,
            currentCost: productDto.CurrentCost,
            reorderLevel: productDto.ReorderLevel,
            sellPrice: productDto.SellPrice
        );

        var productId = await productRepository.CreateOwnProduct(productCompanyEntity);
        return productId;
    }
}