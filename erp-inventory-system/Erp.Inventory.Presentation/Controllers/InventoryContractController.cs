using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.UseCases.Movement;
using Erp.Inventory.Application.UseCases.Product;
using Erp.Inventory.Contracts;
using Erp.Inventory.Domain.Enums;
using Erp.Inventory.Infrastructure.Persistance.Context;
using Erp.Inventory.Infrastructure.Persistance.Models;
using Erp.Inventory.Presentation.Contracts;
using Erp.Inventory.Presentation.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Erp.Inventory.Presentation.Controllers;

[ApiController]
[Route("api/inventory/companies")]
public class InventoryContractController(
    AppDbContext context,
    IInventoryCenResolver cenResolver,
    ICreateOwnProductUseCase createOwnProductUseCase,
    IUpdateOwnProductUseCase updateOwnProductUseCase,
    IDeactivateProductUseCase deactivateProductUseCase,
    IActiveProductUseCase activeProductUseCase,
    ICreateMovementUseCase createMovementUseCase,
    ICreateAdjustmentMovementUseCase createAdjustmentMovementUseCase,
    IInventoryService inventoryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CompanyContractDto>>> GetCompanies()
    {
        var companies = await context.Companies
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.Name)
            .Select(c => new CompanyContractDto
            {
                CompanyCen = c.Cen,
                Name = c.Name,
                IsActive = !c.IsDeleted
            })
            .ToListAsync();

        return Ok(companies);
    }

    [HttpGet("{companyCen}/dashboard")]
    public async Task<ActionResult<InventoryDashboardContractDto>> GetDashboard(string companyCen)
    {
        var companyId = await cenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada." });
        }

        var products = await context.Products
            .Include(p => p.ProductWarehouses)
            .Where(p => p.CompanyId == companyId && !p.IsDeleted)
            .ToListAsync();

        var productStocks = products
            .Select(p => new
            {
                Product = p,
                Stock = p.ProductWarehouses.Where(pw => !pw.IsDeleted).Sum(pw => pw.Quantity)
            })
            .ToList();

        return Ok(new InventoryDashboardContractDto
        {
            CompanyCen = companyCen,
            TotalProducts = products.Count,
            TotalStockQuantity = productStocks.Sum(p => p.Stock),
            LowStockCount = productStocks.Count(p => p.Stock > 0 && p.Stock <= p.Product.ReorderLevel),
            OutOfStockCount = productStocks.Count(p => p.Stock <= 0)
        });
    }

    [HttpGet("{companyCen}/categories")]
    public async Task<ActionResult<List<CategoryContractDto>>> GetCategories(string companyCen)
    {
        var companyId = await cenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada." });
        }

        return Ok(await context.Categories
            .Where(c => c.CompanyId == companyId && !c.IsDeleted)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryContractDto
            {
                CategoryCen = c.Cen,
                Name = c.Name,
                Description = null,
                IsActive = !c.IsDeleted
            })
            .ToListAsync());
    }

    [HttpPost("{companyCen}/categories")]
    public async Task<ActionResult<CategoryContractDto>> CreateCategory(string companyCen, CreateCategoryContractRequest request)
    {
        var companyId = await cenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada." });
        }

        var category = new Category
        {
            Cen = Guid.NewGuid().ToString(),
            Name = request.Name,
            CompanyId = companyId.Value
        };

        context.Categories.Add(category);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategories), new { companyCen }, ToCategoryContract(category));
    }

    [HttpPut("{companyCen}/categories/{categoryCen}")]
    public async Task<IActionResult> UpdateCategory(string companyCen, string categoryCen, CreateCategoryContractRequest request)
    {
        var companyId = await cenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada." });
        }

        var category = await context.Categories
            .FirstOrDefaultAsync(c => c.CompanyId == companyId && c.Cen == categoryCen && !c.IsDeleted);
        if (category is null)
        {
            return NotFound(new { message = "Categoria no encontrada." });
        }

        category.Name = request.Name;
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{companyCen}/units")]
    public async Task<ActionResult<List<UnitContractDto>>> GetUnits(string companyCen)
    {
        var companyId = await cenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada." });
        }

        return Ok(await context.Units
            .Where(u => u.CompanyId == companyId && !u.IsDeleted)
            .OrderBy(u => u.Name)
            .Select(u => new UnitContractDto
            {
                UnitCen = u.Cen,
                Name = u.Name,
                Abbreviation = null,
                IsActive = !u.IsDeleted
            })
            .ToListAsync());
    }

    [HttpPost("{companyCen}/units")]
    public async Task<ActionResult<UnitContractDto>> CreateUnit(string companyCen, CreateUnitContractRequest request)
    {
        var companyId = await cenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada." });
        }

        var unit = new Unit
        {
            Cen = Guid.NewGuid().ToString(),
            Name = request.Name,
            CompanyId = companyId.Value
        };

        context.Units.Add(unit);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUnits), new { companyCen }, ToUnitContract(unit));
    }

    [HttpPut("{companyCen}/units/{unitCen}")]
    public async Task<IActionResult> UpdateUnit(string companyCen, string unitCen, CreateUnitContractRequest request)
    {
        var companyId = await cenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada." });
        }

        var unit = await context.Units
            .FirstOrDefaultAsync(u => u.CompanyId == companyId && u.Cen == unitCen && !u.IsDeleted);
        if (unit is null)
        {
            return NotFound(new { message = "Unidad no encontrada." });
        }

        unit.Name = request.Name;
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{companyCen}/warehouses")]
    public async Task<ActionResult<List<WarehouseContractDto>>> GetWarehouses(string companyCen)
    {
        var companyId = await cenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada." });
        }

        return Ok(await context.Warehouses
            .Where(w => w.CompanyId == companyId && !w.IsDeleted)
            .OrderBy(w => w.Name)
            .Select(w => new WarehouseContractDto
            {
                WarehouseCen = w.Cen,
                Name = w.Name,
                IsActive = !w.IsDeleted
            })
            .ToListAsync());
    }

    [HttpPost("{companyCen}/warehouses")]
    public async Task<ActionResult<WarehouseContractDto>> CreateWarehouse(string companyCen, CreateWarehouseContractRequest request)
    {
        var companyId = await cenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada." });
        }

        var warehouse = new Warehouse
        {
            Cen = Guid.NewGuid().ToString(),
            Name = request.Name,
            CompanyId = companyId.Value
        };

        context.Warehouses.Add(warehouse);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetWarehouses), new { companyCen }, ToWarehouseContract(warehouse));
    }

    [HttpGet("{companyCen}/products")]
    public async Task<ActionResult<List<ProductContractDto>>> GetProducts(
        string companyCen,
        [FromQuery] string? search,
        [FromQuery] string? categoryCen,
        [FromQuery] string? status)
    {
        var companyId = await cenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada." });
        }

        int? categoryId = null;
        if (!string.IsNullOrWhiteSpace(categoryCen))
        {
            categoryId = await cenResolver.ResolveCategoryIdAsync(companyId.Value, categoryCen);
            if (categoryId is null)
            {
                return NotFound(new { message = "Categoria no encontrada." });
            }
        }

        var query = ProductQuery(companyId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.CoreProduct.Name.ToLower().Contains(search.ToLower()) ||
                (p.Sku != null && p.Sku.ToLower().Contains(search.ToLower())) ||
                p.Cen.ToLower().Contains(search.ToLower()));
        }

        if (categoryId is not null)
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        var products = await query.OrderBy(p => p.CoreProduct.Name).ToListAsync();

        if (!string.IsNullOrWhiteSpace(status))
        {
            products = products
                .Where(p => string.Equals(ToContractStatus(p), status, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return Ok(products.Select(ToProductContract).ToList());
    }

    [HttpPost("{companyCen}/products")]
    public async Task<ActionResult<ProductCreatedContractDto>> CreateProduct(string companyCen, CreateProductContractRequest request)
    {
        var companyId = await cenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada." });
        }

        var categoryId = await cenResolver.ResolveCategoryIdAsync(companyId.Value, request.CategoryCen);
        var unitId = await cenResolver.ResolveUnitIdAsync(companyId.Value, request.UnitCen);
        if (categoryId is null || unitId is null)
        {
            return NotFound(new { message = "Categoria o unidad no encontrada." });
        }

        var productStatusId = await GetProductStatusIdAsync("available");
        var supplierId = await context.Suppliers
            .Where(s => s.CompanyId == companyId && !s.IsDeleted)
            .Select(s => (int?)s.Id)
            .FirstOrDefaultAsync();

        if (productStatusId is null || supplierId is null)
        {
            return BadRequest(new { message = "La empresa necesita al menos un estado de producto y un proveedor para crear productos." });
        }

        var productId = await createOwnProductUseCase.ExecuteAsync(new CreateProductDto(
            Name: request.Name,
            ImageUrl: null,
            UnitId: unitId.Value,
            CompanyId: companyId.Value,
            ProductStatusId: productStatusId.Value,
            SupplierId: supplierId.Value,
            CategoryId: categoryId.Value,
            CurrentCost: request.CostPrice ?? 0.01m,
            ReorderLevel: request.ReorderLevel,
            SellPrice: request.SalePrice,
            Sku: request.Sku));

        var product = await ProductQuery(companyId.Value).FirstAsync(p => p.Id == productId);

        return CreatedAtAction(nameof(GetProducts), new { companyCen }, new ProductCreatedContractDto
        {
            ProductCen = product.Cen,
            Sku = product.Sku ?? product.Cen,
            Name = product.CoreProduct.Name,
            Status = ToContractStatus(product),
            InitialStock = 0
        });
    }

    [HttpPut("{companyCen}/products/{productCen}")]
    public async Task<IActionResult> UpdateProduct(string companyCen, string productCen, UpdateProductContractRequest request)
    {
        var companyId = await cenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada." });
        }

        var product = await ProductQuery(companyId.Value)
            .FirstOrDefaultAsync(p => p.Cen == productCen || p.Sku == productCen);
        if (product is null)
        {
            return NotFound(new { message = "Producto no encontrado." });
        }

        var categoryId = await cenResolver.ResolveCategoryIdAsync(companyId.Value, request.CategoryCen);
        var unitId = await cenResolver.ResolveUnitIdAsync(companyId.Value, request.UnitCen);
        if (categoryId is null || unitId is null)
        {
            return NotFound(new { message = "Categoria o unidad no encontrada." });
        }

        await updateOwnProductUseCase.ExecuteAsync(new UpdateProductDto(
            ProductId: product.Id,
            Name: request.Name,
            ImageUrl: product.CoreProduct.ImageUrl,
            UnitId: unitId.Value,
            CompanyId: companyId.Value,
            ProductStatusId: product.ProductStatusId,
            SupplierId: product.SupplierId,
            CategoryId: categoryId.Value,
            CurrentCost: request.CostPrice ?? product.CurrentCost,
            ReorderLevel: request.ReorderLevel,
            SellPrice: request.SalePrice,
            Sku: product.Sku));

        return NoContent();
    }

    [HttpPatch("{companyCen}/products/{productCen}/status")]
    public async Task<IActionResult> UpdateProductStatus(string companyCen, string productCen, UpdateProductStatusContractRequest request)
    {
        var companyId = await cenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada." });
        }

        var productId = await cenResolver.ResolveProductIdAsync(companyId.Value, productCen);
        if (productId is null)
        {
            return NotFound(new { message = "Producto no encontrado." });
        }

        switch (request.Status.Trim().ToUpperInvariant())
        {
            case "ACTIVE":
                await activeProductUseCase.ExecuteAsync(new ActivateProductDto(productId.Value, companyId.Value));
                break;
            case "INACTIVE":
                await deactivateProductUseCase.ExecuteAsync(new DeactivateProductDto(productId.Value, companyId.Value));
                break;
            case "OUT_OF_STOCK":
                var outOfStockStatusId = await GetProductStatusIdAsync("out");
                if (outOfStockStatusId is null)
                {
                    return BadRequest(new { message = "No existe un estado de producto para OUT_OF_STOCK." });
                }

                await context.Products
                    .Where(p => p.Id == productId && p.CompanyId == companyId)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(p => p.IsActive, true)
                        .SetProperty(p => p.ProductStatusId, outOfStockStatusId.Value));
                break;
            default:
                return BadRequest(new { message = "Estado no soportado. Use ACTIVE, INACTIVE u OUT_OF_STOCK." });
        }

        return NoContent();
    }

    [HttpGet("{companyCen}/stock")]
    public async Task<ActionResult<List<StockItemContractDto>>> GetStock(
        string companyCen,
        [FromQuery] string? productCen,
        [FromQuery] string? warehouseCen)
    {
        var companyId = await cenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada." });
        }

        int? productId = null;
        int? warehouseId = null;
        if (!string.IsNullOrWhiteSpace(productCen))
        {
            productId = await cenResolver.ResolveProductIdAsync(companyId.Value, productCen);
            if (productId is null)
            {
                return NotFound(new { message = "Producto no encontrado." });
            }
        }

        if (!string.IsNullOrWhiteSpace(warehouseCen))
        {
            warehouseId = await cenResolver.ResolveWarehouseIdAsync(companyId.Value, warehouseCen);
            if (warehouseId is null)
            {
                return NotFound(new { message = "Almacen no encontrado." });
            }
        }

        var query = context.ProductWarehouses
            .Include(pw => pw.Product).ThenInclude(p => p.CoreProduct)
            .Include(pw => pw.Product).ThenInclude(p => p.Unit)
            .Include(pw => pw.Warehouse)
            .Where(pw => pw.Product.CompanyId == companyId && !pw.Product.IsDeleted && !pw.IsDeleted);

        if (productId is not null)
        {
            query = query.Where(pw => pw.ProductId == productId);
        }

        if (warehouseId is not null)
        {
            query = query.Where(pw => pw.WarehouseId == warehouseId);
        }

        var stock = await query
            .OrderBy(pw => pw.Product.CoreProduct.Name)
            .ThenBy(pw => pw.Warehouse.Name)
            .Select(pw => new StockItemContractDto
            {
                ProductCen = pw.Product.Cen,
                ProductName = pw.Product.CoreProduct.Name,
                WarehouseCen = pw.Warehouse.Cen,
                WarehouseName = pw.Warehouse.Name,
                AvailableQuantity = pw.Quantity,
                ReservedQuantity = 0,
                UnitName = pw.Product.Unit.Name,
                ReorderLevel = pw.Product.ReorderLevel,
                IsLowStock = pw.Quantity > 0 && pw.Quantity <= pw.Product.ReorderLevel
            })
            .ToListAsync();

        return Ok(stock);
    }

    [HttpPost("{companyCen}/stock/validate")]
    public async Task<ActionResult<StockValidationContractResponse>> ValidateStock(string companyCen, StockValidationContractRequest request)
    {
        var validation = await ValidateStockRequestAsync(companyCen, request.WarehouseCen, request.Items);
        if (validation.Error is not null)
        {
            return validation.Error;
        }

        return Ok(validation.Response);
    }

    [HttpPost("{companyCen}/stock/consume")]
    public async Task<ActionResult<StockConsumeContractResponse>> ConsumeStock(string companyCen, StockConsumeContractRequest request)
    {
        var validation = await ValidateStockRequestAsync(companyCen, request.WarehouseCen, request.Items);
        if (validation.Error is not null)
        {
            return validation.Error;
        }

        if (!validation.Response!.IsValid)
        {
            return Conflict(new StockConsumeContractResponse
            {
                Success = false,
                Message = "Stock insuficiente para completar la venta.",
                Requirements = validation.Response.Requirements
            });
        }

        var companyId = validation.CompanyId!.Value;
        var warehouseId = validation.WarehouseId!.Value;
        var documentCen = Guid.NewGuid().ToString();
        var movementCens = request.Items.Select(_ => Guid.NewGuid().ToString()).ToList();
        var today = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");

        await createMovementUseCase.ExecuteAsync(new CreateInventoryMovementDTO
        {
            Cen = documentCen,
            ExternalReference = request.ReferenceCen,
            Title = $"Salida por venta {request.ReferenceCen}",
            MovementDate = today,
            MovementType = (int)MovementTypeEnum.Issue,
            MovementStatus = (int)MovementStatusEnum.Completed,
            CompanyId = companyId,
            Transactions = validation.ResolvedItems!.Zip(movementCens, (item, movementCen) => new CreateTransactionDTO
            {
                Cen = movementCen,
                ProductId = item.ProductId,
                WarehouseId = warehouseId,
                Quantity = -item.Quantity,
                Reason = request.Reason ?? $"Descuento por {request.Source} {request.ReferenceCen}",
                TransactionDate = today,
                TransactionType = (int)TransactionTypeEnum.Out
            }).ToList()
        });

        return Ok(new StockConsumeContractResponse
        {
            Success = true,
            DocumentCen = documentCen,
            DocumentType = "SALE_EXIT",
            GeneratedMovementCens = movementCens
        });
    }

    [HttpPost("{companyCen}/stock/adjustments")]
    public async Task<ActionResult<InventoryDocumentContractDto>> CreateAdjustment(string companyCen, StockAdjustmentContractRequest request)
    {
        var document = new InventoryDocumentContractRequest
        {
            DocumentType = "ADJUSTMENT",
            WarehouseCen = request.WarehouseCen,
            Reason = request.Reason,
            Lines = request.Lines.Select(line => new InventoryDocumentLineContractRequest
            {
                ProductCen = line.ProductCen,
                Quantity = ToAdjustmentQuantity(line),
                UnitCost = null
            }).ToList()
        };

        return await CreateDocumentInternal(companyCen, document, isAdjustment: true);
    }

    [HttpPost("{companyCen}/documents")]
    public async Task<ActionResult<InventoryDocumentContractDto>> CreateDocument(string companyCen, InventoryDocumentContractRequest request)
    {
        return await CreateDocumentInternal(companyCen, request, isAdjustment: false);
    }

    [HttpGet("{companyCen}/documents")]
    public async Task<ActionResult<List<InventoryDocumentContractDto>>> GetDocuments(
        string companyCen,
        [FromQuery] string? documentType,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var companyId = await cenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada." });
        }

        var query = context.InventoryMovements
            .Include(m => m.Transactions)
            .Where(m => m.CompanyId == companyId && !m.IsDeleted);

        if (!string.IsNullOrWhiteSpace(documentType))
        {
            query = documentType.Trim().ToUpperInvariant() switch
            {
                "ENTRY" => query.Where(m => m.MovementTypeId == (int)MovementTypeEnum.Receipt),
                "EXIT" => query.Where(m => m.MovementTypeId == (int)MovementTypeEnum.Issue && m.ExternalReference == null),
                "SALE_EXIT" => query.Where(m => m.MovementTypeId == (int)MovementTypeEnum.Issue && m.ExternalReference != null),
                "ADJUSTMENT" => query.Where(m => m.MovementTypeId == (int)MovementTypeEnum.Adjustment),
                _ => query.Where(_ => false)
            };
        }

        if (from is not null)
        {
            var fromDate = DateOnly.FromDateTime(from.Value);
            query = query.Where(m => m.MovementDate >= fromDate);
        }

        if (to is not null)
        {
            var toDate = DateOnly.FromDateTime(to.Value);
            query = query.Where(m => m.MovementDate <= toDate);
        }

        var movements = await query
            .OrderByDescending(m => m.MovementDate)
            .ToListAsync();

        var documents = movements
            .Select(m => new InventoryDocumentContractDto
            {
                DocumentCen = m.Cen,
                DocumentType = ToDocumentType(m.MovementTypeId, m.ExternalReference),
                Status = ToMovementStatus(m.MovementStatusId),
                CreatedAt = m.MovementDate.ToDateTime(TimeOnly.MinValue),
                TotalItems = m.Transactions.Count,
                GeneratedMovementCens = m.Transactions.Select(t => t.Cen).ToList()
            })
            .ToList();

        return Ok(documents);
    }

    [HttpGet("{companyCen}/products/{productCen}/kardex")]
    public async Task<ActionResult<List<KardexMovementContractDto>>> GetKardex(
        string companyCen,
        string productCen,
        [FromQuery] string? warehouseCen,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var companyId = await cenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada." });
        }

        var productId = await cenResolver.ResolveProductIdAsync(companyId.Value, productCen);
        if (productId is null)
        {
            return NotFound(new { message = "Producto no encontrado." });
        }

        int? warehouseId = null;
        if (!string.IsNullOrWhiteSpace(warehouseCen))
        {
            warehouseId = await cenResolver.ResolveWarehouseIdAsync(companyId.Value, warehouseCen);
            if (warehouseId is null)
            {
                return NotFound(new { message = "Almacen no encontrado." });
            }
        }

        var query = context.Transactions
            .Include(t => t.Product)
            .Include(t => t.Warehouse)
            .Include(t => t.InventoryMovement)
            .Where(t => t.InventoryMovement.CompanyId == companyId
                        && t.ProductId == productId
                        && !t.IsDeleted
                        && !t.InventoryMovement.IsDeleted);

        if (warehouseId is not null)
        {
            query = query.Where(t => t.WarehouseId == warehouseId);
        }

        if (from is not null)
        {
            var fromDate = DateOnly.FromDateTime(from.Value);
            query = query.Where(t => t.TransactionDate >= fromDate);
        }

        if (to is not null)
        {
            var toDate = DateOnly.FromDateTime(to.Value);
            query = query.Where(t => t.TransactionDate <= toDate);
        }

        var transactions = await query
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();

        var kardex = transactions
            .Select(t => new KardexMovementContractDto
            {
                MovementCen = t.Cen,
                DocumentCen = t.InventoryMovement.Cen,
                ProductCen = t.Product.Cen,
                WarehouseCen = t.Warehouse.Cen,
                MovementType = ToMovementType(t.TransactionTypeId),
                Quantity = Math.Abs(t.Quantity),
                UnitCost = t.Product.CurrentCost,
                Reason = t.Reason,
                CreatedAt = t.TransactionDate.ToDateTime(TimeOnly.MinValue)
            })
            .ToList();

        return Ok(kardex);
    }

    private async Task<ActionResult<InventoryDocumentContractDto>> CreateDocumentInternal(
        string companyCen,
        InventoryDocumentContractRequest request,
        bool isAdjustment)
    {
        var companyId = await cenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return NotFound(new { message = "Empresa no encontrada." });
        }

        var warehouseId = await cenResolver.ResolveWarehouseIdAsync(companyId.Value, request.WarehouseCen);
        if (warehouseId is null)
        {
            return NotFound(new { message = "Almacen no encontrado." });
        }

        var resolvedLines = new List<(int ProductId, int Quantity)>();
        foreach (var line in request.Lines)
        {
            var productId = await cenResolver.ResolveProductIdAsync(companyId.Value, line.ProductCen);
            if (productId is null)
            {
                return NotFound(new { message = $"Producto no encontrado: {line.ProductCen}" });
            }

            resolvedLines.Add((productId.Value, ToDocumentQuantity(request.DocumentType, line.Quantity)));
        }

        var isExit = resolvedLines.Any(line => line.Quantity < 0);
        if (isExit)
        {
            var validation = await ValidateStockByIdsAsync(companyId.Value, warehouseId.Value, resolvedLines
                .Where(line => line.Quantity < 0)
                .Select(line => (line.ProductId, Quantity: Math.Abs(line.Quantity)))
                .ToList());
            if (!validation.IsValid)
            {
                return Conflict(new StockConsumeContractResponse
                {
                    Success = false,
                    Message = "Stock insuficiente para registrar el documento.",
                    Requirements = validation.Requirements
                });
            }
        }

        var documentCen = Guid.NewGuid().ToString();
        var movementCens = resolvedLines.Select(_ => Guid.NewGuid().ToString()).ToList();
        var today = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
        var movementType = isAdjustment
            ? MovementTypeEnum.Adjustment
            : isExit ? MovementTypeEnum.Issue : MovementTypeEnum.Receipt;

        var dto = new CreateInventoryMovementDTO
        {
            Cen = documentCen,
            ExternalReference = request.ExternalReference,
            Title = request.Reason ?? $"{request.DocumentType} {documentCen}",
            MovementDate = today,
            MovementType = (int)movementType,
            MovementStatus = (int)MovementStatusEnum.Completed,
            CompanyId = companyId.Value,
            Transactions = resolvedLines.Zip(movementCens, (line, movementCen) => new CreateTransactionDTO
            {
                Cen = movementCen,
                ProductId = line.ProductId,
                WarehouseId = warehouseId.Value,
                Quantity = line.Quantity,
                Reason = request.Reason ?? request.DocumentType,
                TransactionDate = today,
                TransactionType = isAdjustment
                    ? (int)TransactionTypeEnum.Adjustment
                    : line.Quantity < 0 ? (int)TransactionTypeEnum.Out : (int)TransactionTypeEnum.In
            }).ToList()
        };

        if (isAdjustment)
        {
            await createAdjustmentMovementUseCase.ExecuteAsync(dto);
        }
        else
        {
            await createMovementUseCase.ExecuteAsync(dto);
        }

        return Ok(new InventoryDocumentContractDto
        {
            DocumentCen = documentCen,
            DocumentType = isAdjustment ? "ADJUSTMENT" : request.DocumentType.ToUpperInvariant(),
            Status = "REGISTERED",
            CreatedAt = DateTime.UtcNow,
            TotalItems = movementCens.Count,
            GeneratedMovementCens = movementCens
        });
    }

    private async Task<(ActionResult? Error, StockValidationContractResponse? Response, int? CompanyId, int? WarehouseId, List<(int ProductId, int Quantity)>? ResolvedItems)> ValidateStockRequestAsync(
        string companyCen,
        string warehouseCen,
        IReadOnlyList<StockValidationItemContractDto> items)
    {
        var companyId = await cenResolver.ResolveCompanyIdAsync(companyCen);
        if (companyId is null)
        {
            return (NotFound(new { message = "Empresa no encontrada." }), null, null, null, null);
        }

        var warehouseId = await cenResolver.ResolveWarehouseIdAsync(companyId.Value, warehouseCen);
        if (warehouseId is null)
        {
            return (NotFound(new { message = "Almacen no encontrado." }), null, companyId, null, null);
        }

        var resolvedItems = new List<(int ProductId, int Quantity)>();
        foreach (var item in items.Where(i => i.Quantity > 0))
        {
            var productId = await cenResolver.ResolveProductIdAsync(companyId.Value, item.ProductCen);
            if (productId is null)
            {
                return (NotFound(new { message = $"Producto no encontrado: {item.ProductCen}" }), null, companyId, warehouseId, null);
            }

            resolvedItems.Add((productId.Value, item.Quantity));
        }

        var validation = await ValidateStockByIdsAsync(companyId.Value, warehouseId.Value, resolvedItems);
        return (null, validation, companyId, warehouseId, resolvedItems);
    }

    private async Task<StockValidationContractResponse> ValidateStockByIdsAsync(
        int companyId,
        int warehouseId,
        IReadOnlyList<(int ProductId, int Quantity)> items)
    {
        var requirements = items
            .GroupBy(i => i.ProductId)
            .Select(g => new StockRequirementDto
            {
                ProductId = g.Key,
                WarehouseId = warehouseId,
                RequestedQuantity = g.Sum(i => i.Quantity)
            })
            .ToList();

        var result = await inventoryService.ValidateStockAvailabilityAsync(requirements, companyId);

        if (result.AllAvailable)
        {
            return new StockValidationContractResponse
            {
                IsValid = true,
                Requirements = []
            };
        }

        var productIds = result.Insufficiencies.Select(i => i.ProductId).Distinct().ToList();
        var products = await context.Products
            .Include(p => p.Unit)
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);
        var warehouse = await context.Warehouses.FirstAsync(w => w.Id == warehouseId);

        return new StockValidationContractResponse
        {
            IsValid = false,
            Requirements = result.Insufficiencies.Select(i =>
            {
                var product = products.GetValueOrDefault(i.ProductId);
                return new StockRequirementContractDto
                {
                    ProductCen = product?.Cen ?? i.ProductId.ToString(),
                    ProductName = i.ProductName,
                    WarehouseCen = warehouse.Cen,
                    RequestedQuantity = i.RequestedQuantity,
                    AvailableQuantity = i.AvailableQuantity,
                    MissingQuantity = Math.Max(0, i.RequestedQuantity - i.AvailableQuantity),
                    UnitName = product?.Unit.Name,
                    Reason = "INSUFFICIENT_STOCK"
                };
            }).ToList()
        };
    }

    private IQueryable<Product> ProductQuery(int companyId)
    {
        return context.Products
            .Include(p => p.CoreProduct)
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .Include(p => p.ProductStatus)
            .Where(p => p.CompanyId == companyId && !p.IsDeleted);
    }

    private static ProductContractDto ToProductContract(Product product)
    {
        return new ProductContractDto
        {
            ProductCen = product.Cen,
            Sku = product.Sku ?? product.Cen,
            Name = product.CoreProduct.Name,
            Description = null,
            CategoryCen = product.Category.Cen,
            CategoryName = product.Category.Name,
            UnitCen = product.Unit.Cen,
            UnitName = product.Unit.Name,
            SalePrice = product.SellPrice,
            CostPrice = product.CurrentCost,
            ReorderLevel = product.ReorderLevel,
            Status = ToContractStatus(product),
            StationCode = null
        };
    }

    private static CategoryContractDto ToCategoryContract(Category category)
    {
        return new CategoryContractDto
        {
            CategoryCen = category.Cen,
            Name = category.Name,
            Description = null,
            IsActive = !category.IsDeleted
        };
    }

    private static UnitContractDto ToUnitContract(Unit unit)
    {
        return new UnitContractDto
        {
            UnitCen = unit.Cen,
            Name = unit.Name,
            Abbreviation = null,
            IsActive = !unit.IsDeleted
        };
    }

    private static WarehouseContractDto ToWarehouseContract(Warehouse warehouse)
    {
        return new WarehouseContractDto
        {
            WarehouseCen = warehouse.Cen,
            Name = warehouse.Name,
            IsActive = !warehouse.IsDeleted
        };
    }

    private async Task<int?> GetProductStatusIdAsync(string statusNamePart)
    {
        return await context.Statuses
            .Where(s => !s.IsDeleted && s.Name.ToLower().Contains(statusNamePart.ToLower()))
            .OrderBy(s => s.Id)
            .Select(s => (int?)s.Id)
            .FirstOrDefaultAsync()
            ?? await context.Statuses
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.Id)
                .Select(s => (int?)s.Id)
                .FirstOrDefaultAsync();
    }

    private static string ToContractStatus(Product product)
    {
        if (!product.IsActive)
        {
            return "INACTIVE";
        }

        var status = product.ProductStatus.Name.Trim().ToLowerInvariant();
        return status.Contains("out") ? "OUT_OF_STOCK" : "ACTIVE";
    }

    private static string ToMovementType(int transactionTypeId)
    {
        return transactionTypeId switch
        {
            (int)TransactionTypeEnum.In => "ENTRY",
            (int)TransactionTypeEnum.Out => "EXIT",
            (int)TransactionTypeEnum.Adjustment => "ADJUSTMENT",
            _ => "UNKNOWN"
        };
    }

    private static string ToDocumentType(int movementTypeId, string? externalReference)
    {
        return movementTypeId switch
        {
            (int)MovementTypeEnum.Receipt => "ENTRY",
            (int)MovementTypeEnum.Issue when !string.IsNullOrWhiteSpace(externalReference) => "SALE_EXIT",
            (int)MovementTypeEnum.Issue => "EXIT",
            (int)MovementTypeEnum.Adjustment => "ADJUSTMENT",
            _ => "UNKNOWN"
        };
    }

    private static string ToMovementStatus(int movementStatusId)
    {
        return movementStatusId == (int)MovementStatusEnum.Completed ? "REGISTERED" : "DRAFT";
    }

    private static int ToDocumentQuantity(string documentType, int quantity)
    {
        var absoluteQuantity = Math.Abs(quantity);
        return documentType.Trim().ToUpperInvariant() switch
        {
            "ENTRY" => absoluteQuantity,
            "EXIT" or "SALE_EXIT" => -absoluteQuantity,
            "ADJUSTMENT" => quantity,
            _ => quantity
        };
    }

    private static int ToAdjustmentQuantity(StockAdjustmentLineContractRequest line)
    {
        var quantity = Math.Abs(line.Quantity);
        return line.AdjustmentType.Trim().ToUpperInvariant() switch
        {
            "INCREASE" => quantity,
            "DECREASE" => -quantity,
            "SET" => line.Quantity,
            _ => line.Quantity
        };
    }
}
