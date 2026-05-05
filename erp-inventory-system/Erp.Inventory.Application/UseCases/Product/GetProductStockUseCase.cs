using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Application.Mappers;

namespace Erp.Inventory.Application.UseCases.Product;

public class GetProductStockUseCase : IGetProductStockUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly IProductMapper _productMapper;

    public GetProductStockUseCase(IProductRepository productRepository, IProductMapper productMapper)
    {
        this._productRepository = productRepository;
        this._productMapper = productMapper;
    }

    public async Task<List<GetProductStockDTO>> ExecuteAsync(int companyId)
    {
        var productStock = await this._productRepository.GetCompanyProductsWithStockAsync(companyId);
        var productStockDto = productStock.Select(ps => this._productMapper.ProductStockToDto(ps)).ToList();
        
        return productStockDto;
    }
}