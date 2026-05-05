using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Application.Mappers;
using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.UseCases.Product;

public class GetProductCatalogUseCase : IGetProductCatalogUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly IProductMapper _productMapper;

    public GetProductCatalogUseCase(IProductRepository productRepository, IProductMapper productMapper)
    {
        this._productRepository = productRepository;
        this._productMapper = productMapper;
    }
    
    public async Task<List<GetProductCatalogDTO>> ExecuteAsync(int companyId)
    {
        List<ProductEntity> productCatalog = await this._productRepository.GetAll(companyId);
        List<GetProductCatalogDTO> productCatalogDto = productCatalog.Select(pc => this._productMapper.ProductEntityToCatalogDto(pc)).ToList();
     
        //return productCatalogDto;
        return productCatalogDto;
    }
}