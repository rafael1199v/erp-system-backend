using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Application.UseCases.Product;
using Erp.Inventory.Contracts;
using Erp.Inventory.Application.ContractDtos;

namespace Erp.Inventory.Application.ContractAdapters;

public class InventoryProductContractAdapter(
    IInventoryCenResolver cenResolver,
    IInventoryContractMapper mapper,
    IProductRepository productRepository,
    IGetProductCatalogUseCase getProductCatalogUseCase,
    ICreateOwnProductUseCase createOwnProductUseCase,
    IUpdateOwnProductUseCase updateOwnProductUseCase,
    IGetProductWithCompanyUseCase getProductWithCompanyUseCase,
    IActiveProductUseCase activeProductUseCase,
    IDeactivateProductUseCase deactivateProductUseCase) : IInventoryProductContractAdapter
{
    public async Task<InventoryContractResult<List<ProductContractDto>>> GetProductsAsync(
        string companyCen,
        string? search = null,
        string? categoryCen = null,
        string? status = null)
    {
        CenLookup? company = await cenResolver.ResolveCompanyAsync(companyCen);
        if (company is null)
        {
            return InventoryContractResult<List<ProductContractDto>>.NotFound("Empresa no encontrada");
        }

        List<GetProductCatalogDTO> products = await getProductCatalogUseCase.ExecuteAsync(company.Id);
        IEnumerable<ProductContractDto> contractProducts = products.Select(mapper.ToProductContract);

        if (!string.IsNullOrWhiteSpace(search))
        {
            contractProducts = contractProducts.Where(product =>
                product.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                || product.Sku.Contains(search, StringComparison.OrdinalIgnoreCase)
                || product.ProductCen.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(categoryCen))
        {
            contractProducts = contractProducts.Where(product =>
                string.Equals(product.CategoryCen, categoryCen, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            contractProducts = contractProducts.Where(product =>
                string.Equals(product.Status, status, StringComparison.OrdinalIgnoreCase));
        }

        return InventoryContractResult<List<ProductContractDto>>.Ok(contractProducts.ToList());
    }

    public async Task<InventoryContractResult<List<ProductContractDto>>> LookupProductsAsync(
        string companyCen,
        ProductLookupContractRequest request)
    {
        CenLookup? company = await cenResolver.ResolveCompanyAsync(companyCen);
        if (company is null)
        {
            return InventoryContractResult<List<ProductContractDto>>.NotFound("Empresa no encontrada");
        }

        List<string> productCens = (request.ProductCens ?? [])
            .Select(productCen => productCen.Trim())
            .Where(productCen => !string.IsNullOrWhiteSpace(productCen))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (productCens.Count > 100)
        {
            return InventoryContractResult<List<ProductContractDto>>.Invalid("No se pueden consultar mas de 100 productos por lookup");
        }

        if (productCens.Count == 0)
        {
            return InventoryContractResult<List<ProductContractDto>>.Ok([]);
        }

        List<GetProductCatalogDTO> products = await productRepository.GetProductsByCensAsync(company.Id, productCens);
        return InventoryContractResult<List<ProductContractDto>>.Ok(products.Select(mapper.ToProductContract).ToList());
    }

    public async Task<InventoryContractResult<CreateProductContractResponse>> CreateProductAsync(string companyCen, CreateProductContractRequest request)
    {
        CenLookup? company = await cenResolver.ResolveCompanyAsync(companyCen);
        if (company is null)
        {
            return InventoryContractResult<CreateProductContractResponse>.NotFound("Empresa no encontrada");
        }

        CenLookup? category = await cenResolver.ResolveCategoryAsync(company.Id, request.CategoryCen);
        if (category is null)
        {
            return InventoryContractResult<CreateProductContractResponse>.NotFound("Categoria no encontrada");
        }

        CenLookup? unit = await cenResolver.ResolveUnitAsync(company.Id, request.UnitCen);
        if (unit is null)
        {
            return InventoryContractResult<CreateProductContractResponse>.NotFound("Unidad no encontrada");
        }

        int productId = await createOwnProductUseCase.ExecuteAsync(new CreateProductDto(
            Name: request.Name,
            ImageUrl: null,
            UnitId: unit.Id,
            CompanyId: company.Id,
            ProductStatusId: InventoryContractAdapterDefaults.ActiveProductStatusId,
            SupplierId: InventoryContractAdapterDefaults.DefaultSupplierId,
            CategoryId: category.Id,
            CurrentCost: request.CostPrice ?? request.SalePrice,
            ReorderLevel: decimal.ToInt32(decimal.Truncate(request.ReorderLevel)),
            SellPrice: request.SalePrice,
            Sku: request.Sku,
            Description: request.Description,
            StationCode: request.StationCode));

        string productPublicCen = string.IsNullOrWhiteSpace(request.Sku)
            ? (await getProductWithCompanyUseCase.GetProductWithCompanyAsync(productId))?.Cen ?? string.Empty
            : request.Sku;

        return InventoryContractResult<CreateProductContractResponse>.Ok(new CreateProductContractResponse
        {
            ProductCen = productPublicCen,
            Sku = request.Sku,
            Name = request.Name,
            Status = InventoryContractAdapterDefaults.ActiveStatus,
            InitialStock = 0
        });
    }

    public async Task<InventoryContractResult<ProductContractDto>> UpdateProductAsync(string companyCen, string productCen, UpdateProductContractRequest request)
    {
        CenLookup? company = await cenResolver.ResolveCompanyAsync(companyCen);
        if (company is null)
        {
            return InventoryContractResult<ProductContractDto>.NotFound("Empresa no encontrada");
        }

        CenLookup? product = await cenResolver.ResolveProductAsync(company.Id, productCen);
        if (product is null)
        {
            return InventoryContractResult<ProductContractDto>.NotFound("Producto no encontrado");
        }

        CenLookup? category = await cenResolver.ResolveCategoryAsync(company.Id, request.CategoryCen);
        if (category is null)
        {
            return InventoryContractResult<ProductContractDto>.NotFound("Categoria no encontrada");
        }

        CenLookup? unit = await cenResolver.ResolveUnitAsync(company.Id, request.UnitCen);
        if (unit is null)
        {
            return InventoryContractResult<ProductContractDto>.NotFound("Unidad no encontrada");
        }

        var currentProduct = await getProductWithCompanyUseCase.GetProductWithCompanyAsync(product.Id);
        if (currentProduct is null)
        {
            return InventoryContractResult<ProductContractDto>.NotFound("Producto no encontrado");
        }

        await updateOwnProductUseCase.ExecuteAsync(new UpdateProductDto(
            ProductId: product.Id,
            Name: request.Name,
            ImageUrl: currentProduct.ImageUrl,
            UnitId: unit.Id,
            CompanyId: company.Id,
            ProductStatusId: currentProduct.ProductStatusId,
            SupplierId: currentProduct.SupplierId,
            CategoryId: category.Id,
            CurrentCost: request.CostPrice ?? request.SalePrice,
            ReorderLevel: decimal.ToInt32(decimal.Truncate(request.ReorderLevel)),
            SellPrice: request.SalePrice,
            Sku: request.Sku,
            Description: request.Description,
            StationCode: request.StationCode));

        ProductContractDto response = new()
        {
            ProductCen = product.Cen,
            Sku = request.Sku,
            Name = request.Name,
            Description = request.Description,
            CategoryCen = category.Cen,
            CategoryName = category.Name,
            UnitCen = unit.Cen,
            UnitName = unit.Name,
            SalePrice = request.SalePrice,
            CostPrice = request.CostPrice,
            ReorderLevel = request.ReorderLevel,
            Status = currentProduct.ProductStatusId == InventoryContractAdapterDefaults.OutOfStockProductStatusId
                ? InventoryContractAdapterDefaults.OutOfStockStatus
                : InventoryContractAdapterDefaults.ActiveStatus,
            StationCode = request.StationCode
        };

        return InventoryContractResult<ProductContractDto>.Ok(response);
    }

    public async Task<InventoryContractResult<ProductContractDto>> UpdateProductStatusAsync(string companyCen, string productCen, UpdateProductStatusContractRequest request)
    {
        CenLookup? company = await cenResolver.ResolveCompanyAsync(companyCen);
        if (company is null)
        {
            return InventoryContractResult<ProductContractDto>.NotFound("Empresa no encontrada");
        }

        CenLookup? product = await cenResolver.ResolveProductAsync(company.Id, productCen);
        if (product is null)
        {
            return InventoryContractResult<ProductContractDto>.NotFound("Producto no encontrado");
        }

        string normalizedStatus = request.Status.Trim().ToUpperInvariant();
        if (normalizedStatus == InventoryContractAdapterDefaults.ActiveStatus)
        {
            await activeProductUseCase.ExecuteAsync(new ActivateProductDto(ProductId: product.Id, CompanyId: company.Id));
        }
        else if (normalizedStatus == InventoryContractAdapterDefaults.InactiveStatus)
        {
            await deactivateProductUseCase.ExecuteAsync(new DeactivateProductDto(ProductId: product.Id, CompanyId: company.Id));
        }
        else if (normalizedStatus == InventoryContractAdapterDefaults.OutOfStockStatus)
        {
            var currentProduct = await getProductWithCompanyUseCase.GetProductWithCompanyAsync(product.Id);
            if (currentProduct is null)
            {
                return InventoryContractResult<ProductContractDto>.NotFound("Producto no encontrado");
            }

            await updateOwnProductUseCase.ExecuteAsync(new UpdateProductDto(
                ProductId: product.Id,
                Name: currentProduct.Name,
                ImageUrl: currentProduct.ImageUrl,
                UnitId: currentProduct.UnitId,
                CompanyId: company.Id,
                ProductStatusId: InventoryContractAdapterDefaults.OutOfStockProductStatusId,
                SupplierId: currentProduct.SupplierId,
                CategoryId: currentProduct.CategoryId,
                CurrentCost: currentProduct.CurrentCost,
                ReorderLevel: currentProduct.ReorderLevel,
                SellPrice: currentProduct.SellPrice,
                Sku: currentProduct.Sku,
                Description: currentProduct.Description,
                StationCode: currentProduct.StationCode));
        }
        else
        {
            return InventoryContractResult<ProductContractDto>.Invalid("Estado de producto no valido");
        }

        ProductContractDto updatedProduct = (await GetProductsAsync(companyCen))
            .Value?
            .FirstOrDefault(candidate => candidate.ProductCen == product.Cen)
            ?? new ProductContractDto
            {
                ProductCen = product.Cen,
                Sku = product.Cen,
                Name = product.Name,
                Status = normalizedStatus
            };

        return InventoryContractResult<ProductContractDto>.Ok(updatedProduct);
    }

    public async Task<InventoryContractResult<List<SellableProductContractDto>>> GetSellableProductsAsync(
        string companyCen,
        string? search,
        string? categoryCen,
        string? warehouseCen,
        bool onlyAvailable,
        int page,
        int pageSize)
    {
        CenLookup? company = await cenResolver.ResolveCompanyAsync(companyCen);
        if (company is null)
        {
            return InventoryContractResult<List<SellableProductContractDto>>.NotFound("Empresa no encontrada");
        }

        if (!string.IsNullOrWhiteSpace(categoryCen)
            && await cenResolver.ResolveCategoryAsync(company.Id, categoryCen) is null)
        {
            return InventoryContractResult<List<SellableProductContractDto>>.NotFound("Categoria no encontrada");
        }

        if (!string.IsNullOrWhiteSpace(warehouseCen)
            && await cenResolver.ResolveWarehouseAsync(company.Id, warehouseCen) is null)
        {
            return InventoryContractResult<List<SellableProductContractDto>>.NotFound("Almacen no encontrado");
        }

        List<SellableProductContractDto> products = await productRepository.GetSellableProductsAsync(
            company.Id,
            search,
            categoryCen,
            warehouseCen,
            onlyAvailable,
            page,
            pageSize);

        return InventoryContractResult<List<SellableProductContractDto>>.Ok(products);
    }
}
