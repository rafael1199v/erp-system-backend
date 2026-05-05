using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Application.Mappers;
using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.UseCases.Product;

public class GetProductWithWarehouseUseCase : IGetProductWithWarehousesUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly IProductMapper _productMapper;
    
    public GetProductWithWarehouseUseCase(IProductRepository productRepository, IProductMapper productMapper)
    {
        this._productRepository = productRepository;
        this._productMapper = productMapper;
    }
    
    public async Task<List<ProductWarehouseDTO>> ExecuteAsync(int companyId)
    {
        List<ProductEntity> productEntities = await _productRepository.GetAll(companyId);
        List<ProductWarehouseDTO> productsWithWarehouses = productEntities.Select(productEntity => _productMapper.ProductEntityToProductWarehouseDto(productEntity)).ToList();

        return productsWithWarehouses;
    }
}