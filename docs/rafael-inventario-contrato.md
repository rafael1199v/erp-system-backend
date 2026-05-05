# Guia de adaptacion de Inventario de Rafael al contrato comun

Alumno: Rafael Vargas

## 1. Objetivo

Este documento describe como adaptar la API actual de Inventario al contrato comun del curso sin reescribir la implementacion existente. La idea principal es conservar los controladores, casos de uso, repositorios y entidades actuales, y agregar una capa adaptadora que exponga rutas estandar para que otros modulos puedan consumir Inventario sin depender de los detalles internos.

La implementacion actual ya esta cerca del contrato porque:

- Usa el prefijo `api/inventory` en la mayoria de controladores.
- Tiene catalogo de empresas, productos, categorias, unidades y almacenes.
- Tiene consultas de stock.
- Tiene movimientos de inventario.
- Tiene validacion de stock.
- Tiene descuento de stock por pago.

El cambio mas importante es dejar de exponer IDs internos como `companyId`, `productId`, `warehouseId`, `categoryId` y `unitId` en la API publica. El contrato comun debe usar CEN, es decir codigos externos estables, y el backend debe traducir esos CEN a IDs internos antes de llamar a la logica actual.

## 2. Estrategia recomendada

No conviene reemplazar los controladores actuales en una primera etapa. La ruta de menor riesgo es crear controladores adaptadores nuevos con las rutas del contrato comun y mantener los endpoints actuales mientras el frontend y los demas equipos migran.

La forma recomendada es:

1. Agregar columnas `Cen` en las entidades principales.
2. Crear un servicio de resolucion de CEN a IDs internos.
3. Crear DTOs publicos del contrato.
4. Crear controladores adaptadores bajo `/api/inventory/companies/{companyCen}`.
5. Reutilizar los casos de uso actuales desde los adaptadores.
6. Transformar las respuestas para no devolver IDs reales de base de datos.

## 3. Cambios de datos

Agregar columnas externas `Cen` en las tablas principales:

| Entidad actual | Columna nueva | Uso en contrato |
|---|---|---|
| `Company` | `Cen` | `companyCen` |
| `Product` | `Cen` | `productCen` |
| `Category` | `Cen` | `categoryCen` |
| `Unit` | `Cen` | `unitCen` |
| `Warehouse` | `Cen` | `warehouseCen` |
| `InventoryMovement` | `Cen` | `documentCen` o `movementCen` |
| `Transaction` | `Cen` | `movementCen` para kardex por linea |

Los IDs internos deben seguir existiendo para Entity Framework, relaciones y logica interna. El punto clave es que no deben salir en los DTOs publicos del contrato.

Recomendaciones para generar CEN:

- Empresa: `COMP-{id}` al migrar datos existentes, luego permitir un codigo mas estable si el negocio lo define.
- Producto: usar `Product.Cen` como `productCen`; si despues se agrega SKU real, puede mapearse al mismo valor o convivir como campo adicional.
- Categoria: `CAT-{id}` o un codigo basado en nombre.
- Unidad: `UNIT-{id}` o un codigo como `UNIT-U`.
- Almacen: `WH-{id}` o un codigo como `WH-COCINA`.
- Movimiento/documento: `DOC-{yyyy}-{id}` o `MOV-{yyyy}-{id}`.

Para evitar cambios drasticos, estos CEN pueden poblarse inicialmente con una migracion de datos usando los IDs existentes. A partir de ahi, los nuevos registros deben generar su CEN al crearse.

## 4. Servicio de resolucion CEN

Crear un servicio pequeno, por ejemplo `IInventoryCenResolver`, encargado de traducir los CEN publicos a IDs internos.

Responsabilidades minimas:

- Resolver `companyCen` a `companyId`.
- Resolver `productCen` a `productId`, validando que pertenezca a la empresa.
- Resolver `warehouseCen` a `warehouseId`, validando que pertenezca a la empresa.
- Resolver `categoryCen` a `categoryId`, validando que pertenezca a la empresa.
- Resolver `unitCen` a `unitId`, validando que pertenezca a la empresa.
- Devolver `404 Not Found` cuando no exista empresa, producto, almacen, categoria o unidad.

El adaptador debe usar este servicio antes de llamar a los casos de uso actuales. Asi no hay que cambiar inmediatamente todos los use cases que hoy reciben IDs enteros.

## 5. Controladores adaptadores

Crear nuevos endpoints con las rutas del contrato comun. Los controladores actuales pueden quedarse como endpoints internos o de compatibilidad.

Endpoints principales:

| Metodo | Ruta del contrato | Implementacion actual que puede reutilizarse |
|---|---|---|
| `GET` | `/api/inventory/companies` | `CompanyController.GetCompanies` |
| `GET` | `/api/inventory/companies/{companyCen}/dashboard` | `DashboardController.GetLowStock` mas metricas agregadas |
| `GET` | `/api/inventory/companies/{companyCen}/categories` | `CategoryController.GetAllCategoriesByCompany` |
| `POST` | `/api/inventory/companies/{companyCen}/categories` | `CategoryController.CreateCategory` |
| `PUT` | `/api/inventory/companies/{companyCen}/categories/{categoryCen}` | `CategoryController.UpdateCategory` |
| `GET` | `/api/inventory/companies/{companyCen}/units` | `UnitController.GetUnitsByCompany` |
| `POST` | `/api/inventory/companies/{companyCen}/units` | `UnitController.CreateUnit` |
| `PUT` | `/api/inventory/companies/{companyCen}/units/{unitCen}` | `UnitController.UpdateUnit` |
| `GET` | `/api/inventory/companies/{companyCen}/warehouses` | `WarehouseController.GetWarehouses` |
| `GET` | `/api/inventory/companies/{companyCen}/products` | `CompanyController.GetProductCatalog` |
| `POST` | `/api/inventory/companies/{companyCen}/products` | `ProductController.CreateProduct` |
| `PUT` | `/api/inventory/companies/{companyCen}/products/{productCen}` | `ProductController.UpdateProduct` |
| `PATCH` | `/api/inventory/companies/{companyCen}/products/{productCen}/status` | `ProductController.ActivateProduct` y `DeactivateProduct` |
| `GET` | `/api/inventory/companies/{companyCen}/stock` | `CompanyController.GetProductStock` y `ProductController.GetProductsWithWarehouses` |
| `POST` | `/api/inventory/companies/{companyCen}/stock/adjustments` | `MovementController.CreateAdjustmentMovement` |
| `GET` | `/api/inventory/companies/{companyCen}/products/{productCen}/kardex` | `MovementController.GetMovements` filtrado por producto |
| `POST` | `/api/inventory/companies/{companyCen}/documents` | `MovementController.CreateMovement` |
| `GET` | `/api/inventory/companies/{companyCen}/documents` | `MovementController.GetMovements` |
| `POST` | `/api/inventory/companies/{companyCen}/stock/validate` | `ProductController.ValidateProductStock` |
| `POST` | `/api/inventory/companies/{companyCen}/stock/consume` | `MovementController.CreatePaymentMovement` |

Los nuevos controladores deben vivir como fachada del contrato. No deben duplicar reglas de negocio complejas; solo deben traducir entrada y salida.

## 6. DTOs publicos del contrato

Crear DTOs publicos separados de los DTOs internos actuales. Esto evita cambiar de golpe todos los casos de uso.

DTOs minimos:

- `CompanyContractDto`
- `CategoryContractDto`
- `UnitContractDto`
- `WarehouseContractDto`
- `ProductContractDto`
- `CreateProductContractRequest`
- `UpdateProductContractRequest`
- `UpdateProductStatusContractRequest`
- `StockItemContractDto`
- `StockValidationContractRequest`
- `StockValidationContractResponse`
- `StockRequirementContractDto`
- `StockConsumeContractRequest`
- `StockConsumeContractResponse`
- `InventoryDocumentContractRequest`
- `InventoryDocumentContractDto`
- `KardexMovementContractDto`

Reglas para estos DTOs:

- Usar `companyCen`, `productCen`, `warehouseCen`, `categoryCen`, `unitCen`, `documentCen` y `movementCen`.
- No incluir `Id`, `CompanyId`, `ProductId`, `WarehouseId`, `CategoryId` ni `UnitId`.
- Usar nombres y estados del contrato: `ACTIVE`, `INACTIVE`, `OUT_OF_STOCK`, `ENTRY`, `EXIT`, `SALE_EXIT`, `REGISTERED`.
- Mantener `initialStock = 0` al crear productos.

## 7. Mapeos importantes

### Productos

El endpoint actual `POST api/inventory/Product` recibe un DTO con IDs. El adaptador debe recibir:

```json
{
  "sku": "SKU-HAMB-001",
  "name": "Hamburguesa clasica",
  "description": "Hamburguesa con carne, queso y vegetales",
  "categoryCen": "CAT-COMIDA",
  "unitCen": "UNIT-U",
  "salePrice": 35.5,
  "costPrice": 20.0,
  "reorderLevel": 10,
  "stationCode": "KITCHEN"
}
```

Luego debe resolver `companyCen`, `categoryCen` y `unitCen`, armar el `CreateProductDto` interno y llamar al caso de uso actual.

La respuesta publica debe devolver:

```json
{
  "productCen": "SKU-HAMB-001",
  "sku": "SKU-HAMB-001",
  "name": "Hamburguesa clasica",
  "status": "ACTIVE",
  "initialStock": 0
}
```

### Stock

El contrato usa una sola ruta para stock:

```http
GET /api/inventory/companies/{companyCen}/stock
```

Filtros opcionales:

- `productCen`
- `warehouseCen`

El adaptador debe resolver esos filtros a IDs cuando existan y devolver una lista en el formato:

```json
[
  {
    "productCen": "SKU-HAMB-001",
    "productName": "Hamburguesa clasica",
    "warehouseCen": "WH-COCINA",
    "warehouseName": "Cocina",
    "availableQuantity": 25,
    "reservedQuantity": 0,
    "unitName": "Unidad",
    "reorderLevel": 10,
    "isLowStock": false
  }
]
```

### Validacion de stock

El endpoint actual `POST api/inventory/Product/valid-stock` ya valida stock, pero usa IDs internos y responde con `AllAvailable` e `Insufficiencies`.

El contrato requiere:

```http
POST /api/inventory/companies/{companyCen}/stock/validate
```

Entrada:

```json
{
  "warehouseCen": "WH-COCINA",
  "source": "SALES_PAYMENT",
  "referenceCen": "TICKET-2026-0001",
  "items": [
    {
      "productCen": "SKU-HAMB-001",
      "quantity": 3
    }
  ]
}
```

Salida:

```json
{
  "isValid": false,
  "requirements": [
    {
      "productCen": "SKU-HAMB-001",
      "productName": "Hamburguesa clasica",
      "warehouseCen": "WH-COCINA",
      "requestedQuantity": 3,
      "availableQuantity": 1,
      "missingQuantity": 2,
      "unitName": "Unidad",
      "reason": "INSUFFICIENT_STOCK"
    }
  ]
}
```

El adaptador debe convertir:

- `AllAvailable` a `isValid`.
- `Insufficiencies` a `requirements`.
- IDs internos a CEN.

### Consumo de stock por venta

El endpoint actual `POST api/inventory/Movement/payment` puede mapearse a:

```http
POST /api/inventory/companies/{companyCen}/stock/consume
```

Reglas obligatorias:

- Validar stock antes de descontar.
- Si falta stock, devolver `409 Conflict`.
- Si una linea falla, no aplicar descuentos parciales.
- Generar movimiento de salida usando el flujo actual.
- Devolver `documentCen` asociado al movimiento creado.

En una primera etapa, si el caso de uso actual no devuelve el movimiento creado, se debe ajustar para devolver al menos el identificador interno y su `Cen`.

### Documentos y kardex

Para evitar crear una entidad nueva, `InventoryMovement` puede funcionar como documento operativo.

Mapeo recomendado:

| Contrato | Implementacion actual |
|---|---|
| `documentCen` | `InventoryMovement.Cen` |
| `documentType = ENTRY` | `MovementTypeEnum.Entry` |
| `documentType = EXIT` | `MovementTypeEnum.Issue` |
| `documentType = SALE_EXIT` | `MovementTypeEnum.Issue` con razon/referencia de venta |
| `movementCen` | `Transaction.Cen` |

El kardex puede salir de `InventoryMovement` mas sus `Transactions`.

## 8. Orden de implementacion recomendado

### Fase 1: Preparar CEN

- Agregar propiedades `Cen` en modelos EF principales.
- Configurar columnas en `AppDbContext`.
- Crear migracion.
- Poblar CEN para datos existentes.
- Agregar indices unicos donde corresponda, idealmente por empresa para productos, categorias, unidades y almacenes.

### Fase 2: Resolver CEN internamente

- Crear `IInventoryCenResolver`.
- Implementarlo usando `AppDbContext` o repositorios existentes.
- Registrar el servicio en `InventoryModule`.
- Validar pertenencia a empresa en productos, categorias, unidades y almacenes.

### Fase 3: Crear DTOs del contrato

- Agregar DTOs publicos separados de los DTOs internos.
- Mantener los DTOs actuales para no romper casos de uso.
- Crear mapeadores simples de interno a contrato.

### Fase 4: Crear adaptadores de catalogo

- Empresas.
- Dashboard.
- Categorias.
- Unidades.
- Almacenes.
- Productos.
- Estado de productos.

### Fase 5: Crear adaptadores de stock y movimientos

- Stock actual con filtros.
- Ajustes.
- Validacion de stock.
- Consumo por venta.
- Documentos operativos.
- Kardex.

### Fase 6: Migracion gradual

- Mantener endpoints antiguos durante la transicion.
- Documentar endpoints antiguos como internos o legacy.
- Cambiar frontends y consumidores externos hacia las rutas nuevas.
- Cuando todos migren, decidir si se eliminan o se dejan como compatibilidad.

## 9. Pruebas manuales iniciales

Como no existe un proyecto de pruebas automatizadas en el repo, la primera validacion puede hacerse con Postman, Swagger o curl.

Casos minimos:

- `GET /api/inventory/companies` devuelve `companyCen` y no `id`.
- `GET /api/inventory/companies/{companyCen}/products` devuelve `productCen`, `categoryCen` y `unitCen`.
- `POST /api/inventory/companies/{companyCen}/products` crea producto con `initialStock = 0`.
- `GET /api/inventory/companies/{companyCen}/stock` permite filtrar por `productCen` y `warehouseCen`.
- `POST /api/inventory/companies/{companyCen}/stock/validate` devuelve `isValid = true` cuando hay stock.
- `POST /api/inventory/companies/{companyCen}/stock/validate` devuelve `requirements` cuando falta stock.
- `POST /api/inventory/companies/{companyCen}/stock/consume` descuenta stock y genera movimiento.
- `POST /api/inventory/companies/{companyCen}/stock/consume` devuelve `409 Conflict` si falta stock.
- `GET /api/inventory/companies/{companyCen}/products/{productCen}/kardex` devuelve movimientos sin IDs internos.

Despues de validar manualmente, conviene agregar pruebas de integracion para los endpoints adaptadores, porque son la frontera publica que usaran los demas equipos.

## 10. Criterios de aceptacion

La adaptacion se considera lista cuando:

- Todos los endpoints obligatorios del contrato existen bajo `/api/inventory/companies/{companyCen}`.
- Ninguna respuesta publica del contrato expone IDs internos.
- Las rutas antiguas siguen funcionando durante la transicion.
- Productos nuevos se crean con stock inicial `0`.
- Ajustes, entradas, salidas y consumo por venta generan kardex.
- Las salidas no permiten stock negativo.
- La validacion de stock devuelve `requirements` detallados cuando falta stock.
- El consumo por venta es transaccional: si una linea falla, no descuenta ninguna.

## 11. Riesgos y notas

- Si se intenta cambiar todos los use cases para recibir CEN de golpe, la refactorizacion sera mas grande de lo necesario. Es mejor resolver CEN en el adaptador.
- Si se usan CEN derivados de IDs para siempre, se cumple la forma del contrato pero no se logra una identificacion externa realmente estable. Por eso se recomienda agregar columnas reales.
- Si `CreateMovementUseCase` no devuelve el movimiento creado, sera necesario ajustarlo para poder responder `documentCen` y `generatedMovementCens`.
- Si no se agregan indices unicos para CEN, pueden aparecer ambiguedades al resolver productos o almacenes.
- Si se mantiene `InventoryMovement` como documento operativo, hay que ser consistente: todo documento del contrato debe tener un movimiento asociado y todo movimiento que venga del contrato debe tener `Cen`.

## 12. Resultado esperado

Con esta adaptacion, Rafael mantiene su arquitectura actual, pero publica una API estable y comun para sus companeros. Internamente puede seguir usando IDs, repositorios y casos de uso existentes; externamente la API hablara en terminos de CEN, rutas estandar y DTOs del contrato.
