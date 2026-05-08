using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;

namespace Erp.Inventory.Application.UseCases.Product;

public class GetProductWithCompanyUseCase(IProductRepository productRepository) : IGetProductWithCompanyUseCase
{
    public async Task<ProductDto> GetProductWithCompanyAsync(int productId)
    {
        var productWithCompanyEntity = await productRepository.GetProductWithCompanyAsync(productId);
        
        if (productWithCompanyEntity == null)
            throw new Exception($"Product con el Id {productId} no encontrado.");

        return new ProductDto(
            Id: productWithCompanyEntity.Id,
            Cen: productWithCompanyEntity.Cen,
            Sku: productWithCompanyEntity.Sku,
            Name: productWithCompanyEntity.CoreProduct!.Name,
            ImageUrl: productWithCompanyEntity.CoreProduct.ImageUrl,
            UnitId: productWithCompanyEntity.UnitId,
            CompanyId: productWithCompanyEntity.CompanyId,
            CategoryId: productWithCompanyEntity.CategoryId,
            SupplierId: productWithCompanyEntity.SupplierId,
            ProductStatusId: productWithCompanyEntity.ProductStatusId,
            CurrentCost: productWithCompanyEntity.CurrentCost,
            ReorderLevel: productWithCompanyEntity.ReorderLevel,
            SellPrice: productWithCompanyEntity.SellPrice,
            Description: productWithCompanyEntity.Description,
            StationCode: productWithCompanyEntity.StationCode
        );
    }
}
