using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Contracts;
using Erp.Inventory.Domain.Entities;
using Erp.Inventory.Infrastructure.Persistance.Context;
using Erp.Inventory.Infrastructure.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace Erp.Inventory.Infrastructure.Persistance.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }   
    
    public async Task<List<ProductStockEntity>> GetCompanyProductsWithStockAsync(int companyId)
    {
        // var productStock = await _context.ProductWarehouses
        //     .Where(p => p.CompanyId == companyId && !p.IsDeleted)
        //     .Select(p => new ProductStockEntity
        //     {
        //         ProductId = p.Id,
        //         ProductName = p.Name,
        //         Unit = p.Unit,
        //         CurrentCost = p.CurrentCost,
        //         TotalStock = p.ProductWarehouses.Sum(pw => pw.Quantity),
        //         ImageUrl = p.ImageUrl
        //     })
        //     .ToListAsync();
        
        var productStock = await Queryable
            .Where<Product>(_context.Products
                .Include(p => p.CoreProduct)
                .Include(p => p.ProductWarehouses)
                .Include(p => p.Company)
                .Include(p => p.Unit)
                .Include(p => p.Category), p => p.CompanyId == companyId && !p.IsDeleted)
            .ToListAsync();

        return Enumerable.ToList(productStock.Select(ToProductStockEntity));
    }

    public async Task<List<ProductEntity>> GetAll(int companyId)
    {
        var products = await Queryable
            .Where<Product>(_context.Products
                .Include(p => p.CoreProduct)
                .Include(p => p.Category)
                .Include(p => p.Unit)
                .Include(p => p.ProductStatus)
                .Include(p => Enumerable.Where<ProductWarehouse>(p.ProductWarehouses, w => w.Warehouse.CompanyId == companyId))
                .ThenInclude(pw => pw.Warehouse), p => p.CompanyId == companyId && !p.IsDeleted)
            .ToListAsync();
        
        var productEntities = Enumerable.ToList(products.Select(ToDomainEntity));
        
        return productEntities;
    }

    public async Task<ProductEntity> GetById(int productId)
    {
        var product = await Queryable
            .Where<Product>(_context.Products
                .Include(p => p.Category)
                .Include(p => p.Unit)
                .Include(p => p.CoreProduct)
                .Include(p => p.ProductStatus)
                .Include(p => p.ProductWarehouses)
                .ThenInclude(pw => pw.Warehouse), product => product.Id == productId && !product.IsDeleted).FirstOrDefaultAsync();
            
        if(product == null)
            throw new Exception("Product not found");
            
        return ToDomainEntity(product);
    }
    

    public async Task<List<ProductWarehouseStockEntity>> GetStockByIds(List<int> productIds)
    {
        var products = await Queryable
            .Where<Product>(_context.Products
                .Include(pw => pw.ProductWarehouses)
                .ThenInclude(pw => pw.Warehouse), p => productIds.Contains(p.Id) && !p.IsDeleted).ToListAsync();
        
        List<ProductWarehouseStockEntity> productWarehouseStockEntities = new List<ProductWarehouseStockEntity>();

        foreach (var product in products)
        {
            productWarehouseStockEntities.AddRange(this.SplitProductByWarehouses(product));
        }

        return productWarehouseStockEntities;
    }

    public async Task<int> CreateOwnProduct(ProductCompanyEntity productCompanyEntity)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            CoreProduct coreProduct = new()
            {
                Id = productCompanyEntity.CoreProductId,
                Name = productCompanyEntity.CoreProduct!.Name,
                ImageUrl = productCompanyEntity.CoreProduct!.ImageUrl,
            };

            await _context.CoreProducts.AddAsync(coreProduct);
            await _context.SaveChangesAsync();

            var coreProductId = coreProduct.Id;

            Product product = new()
            {
                Cen = ResolveProductCen(productCompanyEntity),
                Sku = productCompanyEntity.Sku,
                Description = productCompanyEntity.Description,
                StationCode = productCompanyEntity.StationCode,
                CoreProductId = coreProductId,
                UnitId = productCompanyEntity.UnitId,
                CompanyId = productCompanyEntity.CompanyId,
                ProductStatusId = productCompanyEntity.ProductStatusId,
                CategoryId = productCompanyEntity.CategoryId,
                SupplierId = productCompanyEntity.SupplierId,
                CurrentCost = productCompanyEntity.CurrentCost,
                ReorderLevel = productCompanyEntity.ReorderLevel,
                SellPrice = productCompanyEntity.SellPrice
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var warehouses = await _context.Warehouses
                .Where(w => w.CompanyId == productCompanyEntity.CompanyId)
                .Select(w => w.Id).ToListAsync();

            var productWarehouses = warehouses.Select(warehouseId => new ProductWarehouse
            {
                ProductId = product.Id,
                WarehouseId = warehouseId,
                Quantity = 0
            });
            
            await _context.ProductWarehouses.AddRangeAsync(productWarehouses);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return product.Id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw new Exception("Hubo un error al crear el producto");
        }
        
    }

    public async Task<bool> UpdateOwnProductAsync(ProductCompanyEntity productCompanyEntity)
    {
        var product = await _context.Products
            .Include(p => p.CoreProduct)
            .FirstOrDefaultAsync(p => p.Id == productCompanyEntity.Id
                                      && p.CompanyId == productCompanyEntity.CompanyId
                                      && !p.IsDeleted);

        if (product == null)
        {
            return false;
        }

        product.CoreProduct.Name = productCompanyEntity.CoreProduct!.Name;
        product.CoreProduct.ImageUrl = productCompanyEntity.CoreProduct.ImageUrl;
        if (!string.IsNullOrWhiteSpace(productCompanyEntity.Cen))
        {
            product.Cen = productCompanyEntity.Cen;
        }

        if (!string.IsNullOrWhiteSpace(productCompanyEntity.Sku))
        {
            product.Sku = productCompanyEntity.Sku;
        }

        if (productCompanyEntity.Description is not null)
        {
            product.Description = productCompanyEntity.Description;
        }

        if (productCompanyEntity.StationCode is not null)
        {
            product.StationCode = productCompanyEntity.StationCode;
        }

        product.UnitId = productCompanyEntity.UnitId;
        product.CategoryId = productCompanyEntity.CategoryId;
        product.ProductStatusId = productCompanyEntity.ProductStatusId;
        product.SupplierId = productCompanyEntity.SupplierId;
        product.CurrentCost = productCompanyEntity.CurrentCost;
        product.ReorderLevel = productCompanyEntity.ReorderLevel;
        product.SellPrice = productCompanyEntity.SellPrice;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeactivateAsync(int productId, int companyId)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId && p.CompanyId == companyId && !p.IsDeleted);

        if (product == null)
        {
            return false;
        }

        product.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AreAllActiveAsync(int companyId, IEnumerable<int> productIds)
    {
        List<int> ids = productIds.Distinct().ToList();
        if (ids.Count == 0)
        {
            return true;
        }

        int activeCount = await Queryable
            .Where<Product>(_context.Products, p => p.CompanyId == companyId
                                                    && !p.IsDeleted
                                                    && p.IsActive
                                                    && ids.Contains(p.Id))
            .CountAsync();

        return activeCount == ids.Count;
    }

    public async Task<bool> ActiveAsync(int productId, int companyId)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId && p.CompanyId == companyId && !p.IsDeleted);

        if (product == null)
        {
            return false;
        }

        product.IsActive = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ProductCompanyEntity?> GetProductWithCompanyAsync(int productId)
    {
        var product = await Queryable
            .Where<Product>(_context.Products
                .Include(p => p.Category)
                .Include(p => p.Unit)
                .Include(p => p.CoreProduct)
                .Include(p => p.ProductStatus)
                .Include(p => p.ProductWarehouses)
                .ThenInclude(pw => pw.Warehouse), product => product.Id == productId && !product.IsDeleted).FirstOrDefaultAsync();

        if (product is null)
            return null;

        return new ProductCompanyEntity
        {
            Id = product.Id,
            Cen = product.Cen,
            Sku = product.Sku,
            Description = product.Description,
            StationCode = product.StationCode,
            CoreProductId = product.CoreProductId,
            UnitId = product.UnitId,
            CompanyId = product.CompanyId,
            ProductStatusId = product.ProductStatusId,
            SupplierId = product.SupplierId,
            CategoryId = product.CategoryId,
            CurrentCost = product.CurrentCost,
            ReorderLevel = product.ReorderLevel,
            SellPrice = product.SellPrice,
            CoreProduct = new CoreProductEntity
            {
                Id = product.CoreProduct.Id,
                Name = product.CoreProduct.Name,
                ImageUrl = product.CoreProduct.ImageUrl
            },
        };
    }

    public async Task<bool> IsProductActiveAsync(int productId, int companyId)
    {
        return await _context.Products
            .Where(p => p.Id == productId && !p.IsDeleted && p.CompanyId == companyId).Select(p => p.IsActive)
            .FirstOrDefaultAsync();
    }

    public async Task<List<RestaurantOrderProductDto>> GetRestaurantOrderProductsAsync(int companyId, int warehouseId)
    {
        return await _context.Products
            .Where(p => p.CompanyId == companyId && !p.IsDeleted && p.IsActive)
            .Select(p => new RestaurantOrderProductDto
            {
                ProductId = p.Id,
                Name = p.CoreProduct.Name,
                SellPrice = p.SellPrice,
                AvailableStock = p.ProductWarehouses
                    .Where(pw => pw.WarehouseId == warehouseId)
                    .Select(pw => pw.Quantity)
                    .FirstOrDefault(),
                IsAvailable = p.ProductWarehouses
                    .Where(pw => pw.WarehouseId == warehouseId)
                    .Select(pw => pw.Quantity)
                    .FirstOrDefault() > 0,
                ProductStatus = p.ProductStatus.Name
            })
            .ToListAsync();
    }

    public async Task<List<RestaurantOrderDetailProductDto>> GetRestaurantOrderDetailProductsAsync(List<int> productIds)
    {
        return await _context.Products
            .Where(p => !p.IsDeleted &&  productIds.Contains(p.Id))
            .Select(p => new RestaurantOrderDetailProductDto
            {
                ProductId = p.Id,
                CategoryId = p.CategoryId,
                Name = p.CoreProduct.Name,
                SellPrice = (double)p.SellPrice,
            })
            .ToListAsync();
    }

    public async Task<List<GetProductCatalogDTO>> GetProductsByCensAsync(int companyId, IEnumerable<string> productCens)
    {
        List<string> normalizedCens = productCens
            .Select(productCen => productCen.Trim())
            .Where(productCen => !string.IsNullOrWhiteSpace(productCen))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalizedCens.Count == 0)
        {
            return [];
        }

        return await _context.Products
            .AsNoTracking()
            .Where(product =>
                product.CompanyId == companyId &&
                !product.IsDeleted &&
                (normalizedCens.Contains(product.Cen) ||
                 (product.Sku != null && normalizedCens.Contains(product.Sku))))
            .OrderBy(product => product.CoreProduct.Name)
            .ThenBy(product => product.Cen)
            .Select(product => new GetProductCatalogDTO
            {
                ProductId = product.Id,
                ProductCen = product.Cen,
                Sku = product.Sku,
                ProductName = product.CoreProduct.Name,
                Description = product.Description,
                Unit = product.Unit.Name,
                UnitCen = product.Unit.Cen,
                CurrentCost = product.CurrentCost,
                SellPrice = product.SellPrice,
                ImageUrl = product.CoreProduct.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryCen = product.Category.Cen,
                CategoryName = product.Category.Name,
                StatusCode = product.ProductStatusId,
                TotalStock = product.ProductWarehouses
                    .Where(stock => !stock.IsDeleted)
                    .Sum(stock => (int?)stock.Quantity) ?? 0,
                ReorderLevel = product.ReorderLevel,
                StationCode = product.StationCode,
                IsActive = product.IsActive
            })
            .ToListAsync();
    }

    public async Task<List<SellableProductContractDto>> GetSellableProductsAsync(
        int companyId,
        string? search,
        string? categoryCen,
        string? warehouseCen,
        bool onlyAvailable,
        int page,
        int pageSize)
    {
        int normalizedPage = Math.Max(page, 1);
        int normalizedPageSize = Math.Clamp(pageSize, 1, 100);
        string? normalizedSearch = string.IsNullOrWhiteSpace(search) ? null : search.Trim().ToLower();
        string? normalizedCategoryCen = string.IsNullOrWhiteSpace(categoryCen) ? null : categoryCen.Trim();
        string? normalizedWarehouseCen = string.IsNullOrWhiteSpace(warehouseCen) ? null : warehouseCen.Trim();

        var products = _context.Products
            .AsNoTracking()
            .Where(product =>
                product.CompanyId == companyId &&
                !product.IsDeleted &&
                product.IsActive &&
                product.ProductStatusId == (int)Domain.Enums.ProductStatus.Available);

        if (normalizedSearch is not null)
        {
            products = products.Where(product =>
                product.CoreProduct.Name.ToLower().Contains(normalizedSearch) ||
                product.Cen.ToLower().Contains(normalizedSearch) ||
                (product.Sku != null && product.Sku.ToLower().Contains(normalizedSearch)));
        }

        if (normalizedCategoryCen is not null)
        {
            products = products.Where(product => product.Category.Cen == normalizedCategoryCen);
        }

        var sellableProducts = products
            .Select(product => new
            {
                product.Cen,
                product.CoreProduct.Name,
                CategoryCen = product.Category.Cen,
                CategoryName = product.Category.Name,
                product.SellPrice,
                AvailableQuantity = product.ProductWarehouses
                    .Where(stock =>
                        !stock.IsDeleted &&
                        stock.Warehouse.CompanyId == companyId &&
                        !stock.Warehouse.IsDeleted &&
                        (normalizedWarehouseCen == null || stock.Warehouse.Cen == normalizedWarehouseCen))
                    .Sum(stock => (int?)stock.Quantity) ?? 0,
                product.StationCode
            });

        if (onlyAvailable)
        {
            sellableProducts = sellableProducts.Where(product => product.AvailableQuantity > 0);
        }

        return await sellableProducts
            .OrderBy(product => product.Name)
            .ThenBy(product => product.Cen)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(product => new SellableProductContractDto
            {
                ProductCen = product.Cen,
                Name = product.Name,
                CategoryCen = product.CategoryCen,
                CategoryName = product.CategoryName,
                SalePrice = product.SellPrice,
                AvailableQuantity = product.AvailableQuantity,
                IsAvailable = product.AvailableQuantity > 0,
                StationCode = product.StationCode
            })
            .ToListAsync();
    }

    public async Task<List<ProductInformationDto>> GetProductInformationAsync(int companyId, List<int> productIds)
    {
        return await _context.Products
            .Where(p => !p.IsDeleted && p.CompanyId == companyId && productIds.Contains(p.Id))
            .Select(p => new ProductInformationDto(Id: p.Id, Name: p.CoreProduct.Name))
            .ToListAsync();
    }


    private ProductEntity ToDomainEntity(Product productModel)
    {
        return new ProductEntity
        {
            ProductId = productModel.Id,
            Cen = productModel.Cen,
            Sku = productModel.Sku,
            ProductName = productModel.CoreProduct.Name,
            Description = productModel.Description,
            StationCode = productModel.StationCode,
            Unit = productModel.Unit.Name,
            UnitCen = productModel.Unit.Cen,
            CurrentCost = productModel.CurrentCost,
            SellPrice = productModel.SellPrice,
            ImageUrl = productModel.CoreProduct.ImageUrl,
            Category = new CategoryEntity
            {
                Id = productModel.CategoryId,
                Cen = productModel.Category.Cen,
                Name = productModel.Category.Name,
                Description = productModel.Category.Description,
                CompanyId = productModel.CompanyId
            },
            Status = MapStatus(productModel),
            Warehouses = productModel.ProductWarehouses.Select(pw => new WarehouseStockEntity
            {
                WarehouseId = pw.WarehouseId,
                WarehouseCen = pw.Warehouse.Cen,
                Stock = pw.Quantity,
                WarehouseName = pw.Warehouse.Name
            }).ToList(),
            ReorderLevel = productModel.ReorderLevel,
            IsActive = productModel.IsActive
        };
    }


    private ProductStockEntity ToProductStockEntity(Product product)
    {
        return new ProductStockEntity
        {
            ProductId = product.Id,
            ProductCen = product.Cen,
            Sku = product.Sku,
            ProductName = product.CoreProduct.Name,
            Unit = product.Unit.Name,
            CurrentCost = product.CurrentCost,
            TotalStock = product.ProductWarehouses.Sum(pw => pw.Quantity),
            ImageUrl = product.CoreProduct.ImageUrl,
        };
    }
    
    private List<ProductWarehouseStockEntity> SplitProductByWarehouses(Product productModel)
    {
        List<ProductWarehouseStockEntity> productWarehouseStockEntities = new List<ProductWarehouseStockEntity>();
        
        foreach (var warehouse in productModel.ProductWarehouses)
        {
            productWarehouseStockEntities.Add(new ProductWarehouseStockEntity
            {
                ProductId = productModel.Id,
                ProductCen = productModel.Cen,
                Sku = productModel.Sku,
                WarehouseId = warehouse.WarehouseId,
                WarehouseCen = warehouse.Warehouse.Cen,
                Stock = warehouse.Quantity,
            });
        }
        
        return productWarehouseStockEntities;
    }

    private Domain.Enums.ProductStatus MapStatus(Product productModel)
    {
        if (!productModel.IsActive)
        {
            return Domain.Enums.ProductStatus.Discontinued;
        }

        string normalizedStatus = productModel.ProductStatus.Name.Trim().ToLowerInvariant();
        return normalizedStatus switch
        {
            "available" => Domain.Enums.ProductStatus.Available,
            "outofstock" => Domain.Enums.ProductStatus.OutOfStock,
            "out_of_stock" => Domain.Enums.ProductStatus.OutOfStock,
            "out of stock" => Domain.Enums.ProductStatus.OutOfStock,
            "discontinued" => Domain.Enums.ProductStatus.Discontinued,
            _ => Domain.Enums.ProductStatus.Unavailable
        };
    }

    private static string ResolveProductCen(ProductCompanyEntity productCompanyEntity)
    {
        if (!string.IsNullOrWhiteSpace(productCompanyEntity.Cen))
        {
            return productCompanyEntity.Cen;
        }

        if (!string.IsNullOrWhiteSpace(productCompanyEntity.Sku))
        {
            return productCompanyEntity.Sku;
        }

        return Guid.NewGuid().ToString();
    }
}
