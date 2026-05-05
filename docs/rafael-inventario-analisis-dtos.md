# Analisis DTO/CEN para adaptar Inventario de Rafael al contrato comun

Alumno: Rafael Vargas

Este documento complementa `docs/rafael-inventario-contrato.md`. Su foco no es repetir el contrato completo, sino analizar los DTOs, rutas y repositorios actuales para decidir como adaptar la API publica sin cambiar drasticamente la implementacion interna.

## 1. Estado actual observado

La API de Inventario ya tiene una base util para el contrato comun:

- Los controladores usan el prefijo `api/inventory`.
- `CompanyController` expone empresas, catalogo de productos y stock por empresa.
- `ProductController` expone creacion, actualizacion, activacion, desactivacion, consultas KDS y validacion de stock.
- `MovementController` expone movimientos, ajustes y descuento por pago.
- `CategoryController`, `UnitController`, `WarehouseController` y `DashboardController` cubren catalogos auxiliares y stock bajo.
- Los repositorios trabajan con IDs internos y Entity Framework, lo cual se debe mantener.

El problema principal es que muchos DTOs que hoy llegan a controladores o salen hacia clientes externos exponen IDs internos. Para cumplir el contrato comun, esos DTOs deben mantenerse como DTOs internos y se debe crear una fachada publica que use CEN.

## 2. Estrategia CEN definitiva

La estrategia recomendada para Rafael es usar CEN reales, no derivados de IDs.

| Entidad | CEN recomendado | Comentario |
|---|---|---|
| `Company` | UUID en `Company.Cen` | Identificador publico estable de empresa. |
| `Category` | UUID en `Category.Cen` | Evita colisiones entre empresas y nombres repetidos. |
| `Unit` | UUID en `Unit.Cen` | Aunque el nombre sea simple, el UUID evita ambiguedad. |
| `Warehouse` | UUID en `Warehouse.Cen` | Permite cambiar nombre del almacen sin romper integraciones. |
| `InventoryMovement` | UUID en `InventoryMovement.Cen` | Puede funcionar como `documentCen`. |
| `Transaction` | UUID en `Transaction.Cen` | Puede funcionar como `movementCen` para kardex por linea. |
| `Product` | SKU como `productCen` si existe; UUID si no existe | Producto es la unica entidad donde un codigo de negocio legible ayuda al cliente. |

Regla practica:

- Para nuevos registros: generar `Guid.NewGuid().ToString()` en el backend, excepto productos cuando el cliente envie `sku`.
- Para productos: si existe `sku`, usarlo como `Product.Cen`; si no existe todavia, agregar `Sku` y mapearlo como CEN publico.
- Para datos existentes: poblar CEN con UUID en migracion. En productos existentes sin SKU, usar UUID inicialmente y permitir completar SKU despues.
- Los IDs internos (`Id`, `CompanyId`, `ProductId`, etc.) deben quedarse para EF, repositorios, relaciones y casos de uso existentes.

## 3. Matriz de DTOs actuales

### Productos

| DTO actual | Uso probable | Campos relevantes | Brecha frente al contrato |
|---|---|---|---|
| `CreateProductDto` | Crear producto interno | `Name`, `ImageUrl`, `UnitId`, `CompanyId`, `ProductStatusId`, `SupplierId`, `CategoryId`, `CurrentCost`, `ReorderLevel`, `SellPrice` | Usa IDs internos. No tiene `Sku`, `ProductCen`, `Description`, `CategoryCen`, `UnitCen`, `StationCode`. |
| `UpdateProductDto` | Actualizar producto interno | `ProductId`, `Name`, `ImageUrl`, IDs relacionados, costos y precio | Usa IDs internos. No recibe `productCen`. Mezcla datos publicos con claves internas. |
| `ProductDto` | Representacion interna de producto | Similar a `UpdateProductDto` | Expone `Id`, `CompanyId`, `CategoryId`, `UnitId`, `SupplierId`, `ProductStatusId`. |
| `GetProductCatalogDTO` | Catalogo por empresa | `ProductId`, `ProductName`, `Unit`, `CurrentCost`, `CategoryId`, `CategoryName`, `StatusCode`, `TotalStock`, `ReorderLevel`, `IsActive` | No tiene `productCen`, `sku`, `categoryCen`, `unitCen`, `salePrice` con nombre del contrato ni `status` textual estandar. |
| `GetProductStockDTO` | Stock agregado por producto | `ProductId`, `ProductName`, `Unit`, `CurrentCost`, `TotalStock`, `ImageUrl` | No incluye almacen, `warehouseCen`, `availableQuantity`, `reservedQuantity`, `reorderLevel` ni `isLowStock`. |

Observacion importante: `ProductRepository.CreateOwnProduct` ya crea stock `0` en todos los almacenes de la empresa mediante `ProductWarehouse`. Eso encaja con el contrato: crear producto no debe cargar stock inicial.

### Catalogos auxiliares

| DTO actual | Uso probable | Campos relevantes | Brecha frente al contrato |
|---|---|---|---|
| `GetCompanyDTO` | Listar empresas | `Id`, `Name` | Debe devolver `companyCen`, `name`, `isActive`. |
| `CategoryDto` | Listar/actualizar categorias | `Id`, `Name`, `CompanyId` | Debe usar `categoryCen`; falta `description` e `isActive`. |
| `CreateCategoryDto` | Crear categoria | `Name`, `CompanyId` | La empresa debe venir por `companyCen` en ruta, no en body. Falta `description`. |
| `UnitDto` | Listar/actualizar unidades | `Id`, `Name`, `CompanyId` | Debe usar `unitCen`; falta `abbreviation` e `isActive`. |
| `CreateUnitDto` | Crear unidad | `Name`, `CompanyId` | La empresa debe venir por ruta. Falta `abbreviation`. |
| `WarehouseDTO` | Listar almacenes | `Id`, `Name` | Debe usar `warehouseCen` y `isActive`. |
| `SupplierDto` | Listar proveedores | `Id`, `Name` | No es obligatorio para el contrato minimo, pero si se expone debe ocultar IDs internos. |

### Stock y dashboard

| DTO actual | Uso probable | Campos relevantes | Brecha frente al contrato |
|---|---|---|---|
| `ProductWarehouseDTO` | Producto con stock por almacen | `Product`, `Warehouses` | Reutilizable, pero sus hijos exponen IDs. |
| `GetWarehouseWithStockDTO` | Stock por almacen | `Id`, `Name`, `Stock` | Debe usar `warehouseCen`, `warehouseName`, `availableQuantity`. |
| `CurrentStockDto` | Stock interno para validacion | `ProductId`, `WarehouseId`, `Quantity` | Correcto como DTO interno; no debe salir como contrato publico. |
| `LowStockDashboardDto` | Dashboard stock bajo | `ProductId`, `ProductName`, `TotalStock`, `ReorderLevel`, `StockState` | Debe ocultar `ProductId` y mapear estado a contrato si se expone. |

El contrato pide que `GET /api/inventory/companies/{companyCen}/stock` devuelva una lista por producto y almacen. La implementacion actual tiene stock agregado por producto y tambien producto con almacenes, por lo que la fachada puede construir el DTO publico sin cambiar toda la consulta desde el primer dia.

### Movimientos, documentos y kardex

| DTO actual | Uso probable | Campos relevantes | Brecha frente al contrato |
|---|---|---|---|
| `CreateInventoryMovementDTO` | Crear entrada/salida/ajuste | `Title`, `MovementDate`, `MovementType`, `MovementStatus`, `CompanyId`, `Transactions` | Usa IDs internos y enums numericos. Falta `documentType`, `warehouseCen`, `externalReference`, `documentCen`. |
| `InventoryMovementDTO` | Listar movimientos | `Id`, `Title`, `MovementDate`, `MovementType`, `MovementStatus`, `Transactions` | Expone `Id`, usa enums numericos y no tiene `documentCen`. |
| `CreateTransactionDTO` | Linea de movimiento | `Quantity`, `Reason`, `TransactionDate`, `TransactionType`, `ProductId`, `WarehouseId` | Usa IDs internos. Falta `productCen`, `warehouseCen`, `unitCost`. |
| `TransactionDTO` | Linea de movimiento en respuesta | `Id`, `Quantity`, `Reason`, `TransactionDate`, `TransactionType`, `ProductId`, `WarehouseId` | Expone IDs internos y no tiene `movementCen`, `documentCen`, nombres ni CEN. |
| `TransactionDetailsDTO` | Detalle por producto | `ProductId`, `ProductName`, `Transactions` | Debe usar `productCen` y movimientos con CEN. |

La entidad `InventoryMovement` puede seguir funcionando como documento operativo. La diferencia es que la fachada debe traducir:

- `documentType = ENTRY` a `MovementTypeEnum.Entry`.
- `documentType = EXIT` a `MovementTypeEnum.Issue`.
- `documentType = SALE_EXIT` a `MovementTypeEnum.Issue` con referencia de venta.
- `InventoryMovement.Cen` a `documentCen`.
- `Transaction.Cen` a `movementCen`.

### DTOs compartidos con ventas

| DTO actual | Uso probable | Campos relevantes | Brecha frente al contrato |
|---|---|---|---|
| `StockValidationDto` | Validar stock desde ventas | `CompanyId`, `Requirements` | Debe recibir `companyCen` por ruta, `warehouseCen`, `source`, `referenceCen`, `items`. |
| `StockRequirementDto` | Requerimiento de stock | `ProductId`, `RequestedQuantity`, `WarehouseId` | Debe usar `productCen` y `quantity`; `warehouseCen` debe estar en request o por linea si se requiere. |
| `StockValidationResultDto` | Resultado de validacion | `AllAvailable`, `Insufficiencies` | El contrato pide `isValid` y `requirements`. |
| `StockInsufficiencyDto` | Faltante de stock | `ProductId`, `ProductName`, `RequestedQuantity`, `AvailableQuantity` | Falta `productCen`, `warehouseCen`, `missingQuantity`, `unitName`, `reason`. |
| `CreatePaymentStockDiscountDto` | Descuento por pago | `RestaurantOrderId`, `CompanyId`, `WarehouseId`, `PaymentDateUtc`, `Items` | Debe recibir `referenceCen`, `companyCen`, `warehouseCen`, `source`, `reason`, `items` con `productCen`. |
| `PaymentStockDiscountItemDto` | Item de descuento | `ProductId`, `Quantity`, `Reason` | Debe usar `productCen`. |
| `RestaurantOrderProductDto` | Productos para ventas/KDS | `ProductId`, `Name`, `SellPrice`, `AvailableStock`, `IsAvailable`, `ProductStatus` | Debe ocultar `ProductId`; puede mapearse a catalogo de productos del contrato. |
| `RestaurantOrderDetailProductDto` | Detalle de productos para ventas | `ProductId`, `CategoryId`, `Name`, `SellPrice` | Debe usar `productCen` y `categoryCen`. |

## 4. Incoherencias y riesgos detectados

1. Los DTOs de aplicacion estan actuando como DTOs internos y publicos a la vez. Eso hace que los IDs internos salgan por la API.
2. El contrato necesita CEN, pero los modelos actuales no tienen `Cen`, `Sku` ni `ExternalReference`.
3. `CreateProductDto` no permite enviar SKU. Si producto debe usar SKU como `productCen`, hay que agregar `Sku` a producto o crear un DTO adaptador que lo reciba y lo persista.
4. `CategoryDto` y `UnitDto` no tienen `description`, `abbreviation` ni `isActive`. Si no se quiere migrar todo ahora, la fachada puede devolver `description = null`, `abbreviation = null` e `isActive = true` mientras se agregan columnas reales.
5. La validacion de stock ya existe, pero responde con nombres distintos al contrato: `AllAvailable` e `Insufficiencies`.
6. El consumo por pago descuenta stock, pero `ExecutePaymentStockDiscountAsync` no devuelve `documentCen` ni movimientos generados.
7. `CreateMovementUseCase` y `CreateAdjustmentMovementUseCase` no devuelven el movimiento creado. Para cumplir bien `stock/consume` y `documents`, conviene devolver un resultado con `documentCen`.
8. `UpdateStockUseCase` protege stock negativo a traves del dominio, pero el contrato requiere una respuesta previa con `requirements`; por eso `stock/consume` debe validar antes de llamar al movimiento.
9. Los movimientos usan enums numericos. El contrato debe exponer textos (`ENTRY`, `EXIT`, `SALE_EXIT`, `REGISTERED`) para que otros equipos no dependan de tus codigos internos.
10. Los errores actuales suelen devolver `400 BadRequest` con mensaje. El contrato necesita distinguir `404` para CEN inexistente y `409` para stock insuficiente al consumir.

## 5. DTOs publicos recomendados para la fachada

Estos DTOs deben vivir separados de los DTOs internos actuales. Una ubicacion razonable es `Erp.Inventory.Presentation/Contracts` o un namespace similar para DTOs publicos del adaptador.

### Catalogo

```csharp
public sealed class CompanyContractDto
{
    public required string CompanyCen { get; init; }
    public required string Name { get; init; }
    public bool IsActive { get; init; }
}

public sealed class CategoryContractDto
{
    public required string CategoryCen { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
}

public sealed class UnitContractDto
{
    public required string UnitCen { get; init; }
    public required string Name { get; init; }
    public string? Abbreviation { get; init; }
    public bool IsActive { get; init; }
}

public sealed class WarehouseContractDto
{
    public required string WarehouseCen { get; init; }
    public required string Name { get; init; }
    public bool IsActive { get; init; }
}
```

### Productos

```csharp
public sealed class ProductContractDto
{
    public required string ProductCen { get; init; }
    public required string Sku { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string CategoryCen { get; init; }
    public required string CategoryName { get; init; }
    public required string UnitCen { get; init; }
    public required string UnitName { get; init; }
    public decimal SalePrice { get; init; }
    public decimal? CostPrice { get; init; }
    public int ReorderLevel { get; init; }
    public required string Status { get; init; }
    public string? StationCode { get; init; }
}

public sealed class CreateProductContractRequest
{
    public required string Sku { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string CategoryCen { get; init; }
    public required string UnitCen { get; init; }
    public decimal SalePrice { get; init; }
    public decimal? CostPrice { get; init; }
    public int ReorderLevel { get; init; }
    public string? StationCode { get; init; }
}
```

### Stock y ventas

```csharp
public sealed class StockValidationContractRequest
{
    public required string WarehouseCen { get; init; }
    public required string Source { get; init; }
    public string? ReferenceCen { get; init; }
    public required IReadOnlyList<StockValidationItemContractDto> Items { get; init; }
}

public sealed class StockValidationItemContractDto
{
    public required string ProductCen { get; init; }
    public int Quantity { get; init; }
}

public sealed class StockValidationContractResponse
{
    public bool IsValid { get; init; }
    public required IReadOnlyList<StockRequirementContractDto> Requirements { get; init; }
}

public sealed class StockRequirementContractDto
{
    public required string ProductCen { get; init; }
    public required string ProductName { get; init; }
    public required string WarehouseCen { get; init; }
    public int RequestedQuantity { get; init; }
    public int AvailableQuantity { get; init; }
    public int MissingQuantity { get; init; }
    public string? UnitName { get; init; }
    public required string Reason { get; init; }
}
```

### Documentos y kardex

```csharp
public sealed class InventoryDocumentContractRequest
{
    public required string DocumentType { get; init; }
    public required string WarehouseCen { get; init; }
    public string? Reason { get; init; }
    public string? ExternalReference { get; init; }
    public required IReadOnlyList<InventoryDocumentLineContractRequest> Lines { get; init; }
}

public sealed class InventoryDocumentLineContractRequest
{
    public required string ProductCen { get; init; }
    public int Quantity { get; init; }
    public decimal? UnitCost { get; init; }
}

public sealed class KardexMovementContractDto
{
    public required string MovementCen { get; init; }
    public string? DocumentCen { get; init; }
    public required string ProductCen { get; init; }
    public required string WarehouseCen { get; init; }
    public required string MovementType { get; init; }
    public int Quantity { get; init; }
    public decimal? UnitCost { get; init; }
    public string? Reason { get; init; }
    public DateTime CreatedAt { get; init; }
}
```

## 6. Mapeo desde DTOs actuales hacia contrato

| Contrato | Fuente actual | Transformacion necesaria |
|---|---|---|
| `CompanyContractDto` | `GetCompanyDTO` | Resolver `Id` a `Company.Cen`; `IsActive = !IsDeleted`. |
| `ProductContractDto` | `GetProductCatalogDTO` | Resolver `ProductId`, `CategoryId`, unidad por nombre a CEN; mapear `SellPrice` como `salePrice`. |
| `StockItemContractDto` | `ProductWarehouseDTO` + `GetWarehouseWithStockDTO` | Resolver producto y almacen a CEN; calcular `isLowStock`. |
| `StockValidationContractResponse` | `StockValidationResultDto` | `IsValid = AllAvailable`; mapear `Insufficiencies` a `Requirements`. |
| `StockConsumeContractResponse` | `CreatePaymentStockDiscountDto` + movimiento creado | Requiere que el caso de uso devuelva o permita consultar `documentCen`. |
| `InventoryDocumentContractDto` | `InventoryMovementDTO` | Mapear movimiento a documento, enum numerico a texto y transacciones a `generatedMovementCens`. |
| `KardexMovementContractDto` | `TransactionDTO` dentro de `InventoryMovementDTO` | Resolver producto, almacen, documento y transaccion a CEN. |

## 7. Orden de implementacion recomendado

1. Agregar columnas `Cen` con UUID en entidades centrales y `Sku` en producto.
2. Poblar datos existentes con UUID; para productos, usar SKU si existe o UUID si todavia no existe.
3. Crear resolver `IInventoryCenResolver` para traducir CEN a IDs internos.
4. Crear DTOs publicos de fachada sin IDs internos.
5. Crear mapeadores de DTO interno a DTO publico.
6. Exponer controladores adaptadores bajo `/api/inventory/companies/{companyCen}`.
7. Ajustar validacion/consumo para responder `requirements`, `409 Conflict`, `documentCen` y `generatedMovementCens`.
8. Mantener controladores actuales como compatibilidad mientras los consumidores migran.

## 8. Checklist de pruebas manuales

- `GET /api/inventory/companies` devuelve UUID en `companyCen` y no devuelve `id`.
- `GET /api/inventory/companies/{companyCen}/products` devuelve `productCen` como SKU cuando existe.
- Crear producto con `sku` genera `Product.Cen = sku` y stock `0` en almacenes.
- Categorias, unidades y almacenes devuelven UUID en sus CEN.
- Validacion de stock devuelve `isValid = false` y `requirements` con `missingQuantity`.
- Consumo de stock con faltantes devuelve `409 Conflict` y no descuenta ninguna linea.
- Consumo exitoso devuelve `documentCen` y `generatedMovementCens`.
- Kardex no expone `Transaction.Id`, `ProductId` ni `WarehouseId`.
- Los endpoints antiguos siguen funcionando durante la transicion.

## 9. Conclusion

La adaptacion no necesita reemplazar la arquitectura actual. Los DTOs actuales pueden seguir vivos como DTOs internos para casos de uso y repositorios. La API publica debe tener una fachada con DTOs propios, CEN por UUID para entidades centrales y SKU como `productCen` para productos cuando aplique. Esta separacion permite cumplir el contrato comun sin romper la logica existente de Inventario ni la integracion actual con Ventas.
