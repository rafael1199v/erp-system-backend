# Examen Parcial — Rafael Andres Vargas Mamani

## Sección 1 — Identificación

- **Nombre completo:** Rafael Andres Vargas Mamani
- **Pareja asignada para el sábado:** Enrique Díaz
- **Repositorio de Inventario:** [Repositorio de inventario](https://github.com/rafael1199v/erp-system-backend/tree/main/erp-sales-system)
- **Repositorio de Ventas:** [Repositorio de ventas](https://github.com/rafael1199v/erp-system-backend/tree/main/erp-inventory-system)
- **Contrato API acordado en grupo:** [link al archivo contrato-api.yaml en este repo]
- **URL del Swagger autogenerado** (cuando levantás el backend localmente): 
    - http://localhost:5143/swagger/index.html
    - http://localhost:5074/swagger/index.html

## Sección 2 — Decisiones técnicas con snippets

### 2.1 Árbol de carpetas del backend de Ventas

```text             
+---Erp.Sales.Application
|   |   Erp.Sales.Application.csproj
|   |   
|   |               
|   +---ContractDtos
|   |       AssignTicketWaiterContractRequest.cs
|   |       AssignTicketWaiterContractResponse.cs
|   |       CancelTicketContractRequest.cs
|   |       CancelTicketContractResponse.cs
|   |       CreateTicketContractRequest.cs
|   |       CreateTicketItemContractRequest.cs
|   |       KdsItemContractResponse.cs
|   |       KdsTeamContractResponse.cs
|   |       PaymentMethodContractResponse.cs
|   |       PayTicketContractRequest.cs
|   |       PayTicketContractResponse.cs
|   |       TaxConfigurationContractResponse.cs
|   |       TicketContractResponse.cs
|   |       TicketItemContractResponse.cs
|   |       TicketTotalsContractResponse.cs
|   |       TopProductDashboardContractResponse.cs
|   |       UpdateKdsItemStatusContractRequest.cs
|   |       UpdateTaxConfigurationContractRequest.cs
|   |       UpdateTicketItemContractRequest.cs
|   |       WaiterContractResponse.cs
|   |       
|   +---ContractServices
|   |       ISalesDashboardContractService.cs
|   |       ITicketContractService.cs
|   |       SalesDashboardContractService.cs
|   |       TicketContractService.cs
|   |       
|   +---DTOs
|   |       AssignWaiterDto.cs
|   |       CancelRestaurantOrderDto.cs
|   |       CreateGlobalTaxDto.cs
|   |       CreateRestaurantOrderDetail.cs
|   |       CreateRestaurantOrderDto.cs
|   |       DailySalesDashboardDto.cs
|   |       KdsOrderItem.cs
|   |       KdsStatusDashboardDto.cs
|   |       KdsTeamDto.cs
|   |       PaymentTypeDto.cs
|   |       ProcessRestaurantOrderPaymentDto.cs
|   |       ProcessRestaurantOrderPaymentResultDto.cs
|   |       RestaurantOrderDetailDto.cs
|   |       RestaurantOrderDto.cs
|   |       StockInsufficiencyResponseDto.cs
|   |       TicketPrintDto.cs
|   |       TicketTotalsDto.cs
|   |       TopProductDashboardDto.cs
|   |       UpdateGlobalTaxDto.cs
|   |       UpdateKdsItemDto.cs
|   |       UpdateRestaurantOrderDetailQuantityDto.cs
|   |       UpsertGlobalTaxDto.cs
|   |       WaiterDto.cs
|   |       WaiterOptionDto.cs
|   |       
|   +---Interfaces
|   |       IDashboardRepository.cs
|   |       IKdsRepository.cs
|   |       IOrderRepository.cs
|   |       IPaymentProcessRepository.cs
|   |       IPaymentTypeRepository.cs
|   |       IRestaurantOrderDetailRepository.cs
|   |       IRestaurantOrderRepository.cs
|   |       ISalesCenResolver.cs
|   |       ITaxConfigurationRepository.cs
|   |       IWaiterRepository.cs
|   |       IWarehouseConfigurationRepository.cs
|   |       
|   +---Services
|   |       IPdfService.cs
|   |       PaymentMethodCodeNormalizer.cs
|   |       
|   \---UseCases
|       +---Dashboard
|       |       BoliviaDateRangeHelper.cs
|       |       GetDailySalesDashboardUseCase.cs
|       |       GetKdsStatusDashboardUseCase.cs
|       |       GetTopProductsDashboardUseCase.cs
|       |       IGetDailySalesDashboardUseCase.cs
|       |       IGetKdsStatusDashboardUseCase.cs
|       |       IGetTopProductsDashboardUseCase.cs
|       |       
|       +---KDS
|       |       ChangeKdsItemStatusUseCase.cs
|       |       GetKdsTeamItemsUseCase.cs
|       |       GetKdsTeamsUseCase.cs
|       |       IChangeKdsItemStatusUseCase.cs
|       |       IGetKdsTeamItemsUseCase.cs
|       |       IGetKdsTeamsUseCase.cs
|       |       
|       +---RestaurantOrder
|       |       AssignWaiterUseCase.cs
|       |       CancelRestaurantOrderUseCase.cs
|       |       CreateRestaurantOrderDetailUseCase.cs
|       |       CreateRestaurantOrderUseCase.cs
|       |       GetOrderDetailProductsUseCase.cs
|       |       GetPaymentTypesUseCase.cs
|       |       GetRestaurantOrderDetailsUseCase.cs
|       |       GetRestaurantOrdersUseCase.cs
|       |       GetRestaurantOrderTaxUseCase.cs
|       |       GetTicketTotalsUseCase.cs
|       |       IAssignWaiterUseCase.cs
|       |       ICancelRestaurantOrderUseCase.cs
|       |       ICreateRestaurantOrderDetailUseCase.cs
|       |       ICreateRestaurantOrderUseCase.cs
|       |       IGetOrderDetailProductsUseCase.cs
|       |       IGetPaymentTypesUseCase.cs
|       |       IGetRestaurantOrderDetailsUseCase.cs
|       |       IGetRestaurantOrdersUseCase.cs
|       |       IGetRestaurantOrderTaxUseCase.cs
|       |       IGetTicketTotalsUseCase.cs
|       |       IPrintRestaurantOrderUseCase.cs
|       |       IPrintTicketContractUseCase.cs
|       |       IProcessRestaurantOrderPaymentUseCase.cs
|       |       ISendOrderUseCase.cs
|       |       IUpdateRestaurantOrderDetailQuantityUseCase.cs
|       |       PrintRestaurantOrderUseCase.cs
|       |       PrintTicketContractUseCase.cs
|       |       ProcessRestaurantOrderPaymentUseCase.cs
|       |       SendOrderUseCase.cs
|       |       UpdateRestaurantOrderDetailQuantityUseCase.cs
|       |       
|       +---RestaurantOrderDetails
|       |       IResendOrderDetailUseCase.cs
|       |       ResendOrderDetailUseCase.cs
|       |       
|       +---TaxConfiguration
|       |       CreateGlobalTaxUseCase.cs
|       |       GetGlobalTaxUseCase.cs
|       |       GetTaxConfigurationUseCase.cs
|       |       ICreateGlobalTaxUseCase.cs
|       |       IGetGlobalTaxUseCase.cs
|       |       IGetTaxConfigurationUseCase.cs
|       |       IUpdateGlobalTaxUseCase.cs
|       |       IUpsertGlobalTaxUseCase.cs
|       |       UpdateGlobalTaxUseCase.cs
|       |       UpsertGlobalTaxUseCase.cs
|       |       
|       \---Waiters
|               GetWaiterOptionsByCompanyUseCase.cs
|               GetWaitersByCompanyUseCase.cs
|               IGetWaiterOptionsByCompanyUseCase.cs
|               IGetWaitersByCompanyUseCase.cs
|               
+---Erp.Sales.Domain
|   |   Erp.Sales.Domain.csproj
|   |               
|   +---Entities
|   |       GlobalTaxConfiguration.cs
|   |       KdsOrderDetail.cs
|   |       KdsTeam.cs
|   |       Order.cs
|   |       PaymentTypeOption.cs
|   |       RestaurantOrder.cs
|   |       RestaurantOrderDetail.cs
|   |       Sale.cs
|   |       SaleDetail.cs
|   |       Waiter.cs
|   |       
|   +---Enums
|   |       OrderDetailStatus.cs
|   |       OrderStatus.cs
|   |       PaymentType.cs
|   |                          
+---Erp.Sales.Infrastructure
|   |   Erp.Sales.Infrastructure.csproj
|   |   Erp.Sales.Infrastructure.csproj.lscache
|   |   SalesDbContext.cs
|   |   Schemas.cs
|   |                     
|   +---Http
|   |       InventoryHttpClient.cs
|   |       
|   +---Migrations
|   |       20260520014031_Init.cs
|   |       20260520014031_Init.Designer.cs
|   |       SalesDbContextModelSnapshot.cs
|   |       
|   +---Models
|   |   |   CustomerModel.cs
|   |   |   OrderDetailModel.cs
|   |   |   OrderModel.cs
|   |   |   OrderStatusModel.cs
|   |   |   PaymentTypeModel.cs
|   |   |   SaleDetailModel.cs
|   |   |   SaleModel.cs
|   |   |   TaxConfigurationModel.cs
|   |   |   WarehouseConfigurationModel.cs
|   |   |   
|   |   \---PoS
|   |           RestaurantOrderDetailModel.cs
|   |           RestaurantOrderDetailStatusModel.cs
|   |           RestaurantOrderModel.cs
|   |           TeamConfigurationModel.cs
|   |           TeamModel.cs
|   |           WaiterModel.cs                  
|   +---Pdf
|   |       PdfService.cs
|   |       
|   +---Repositories
|   |       DashboardRepository.cs
|   |       KdsRepository.cs
|   |       OrderRepository.cs
|   |       PaymentProcessRepository.cs
|   |       PaymentTypeRepository.cs
|   |       RestaurantOrderDetailRepository.cs
|   |       RestaurantOrderRepository.cs
|   |       TaxConfigurationRepository.cs
|   |       WaiterRepository.cs
|   |       WarehouseConfigurationRepository.cs
|   |       
|   \---Services
|           SalesCenResolver.cs
|           
+---Erp.Sales.Presentation
|   |   Erp.Sales.Presentation.csproj
|   |   Erp.Sales.Presentation.csproj.lscache
|   |   SalesModule.cs
|   |                      
|   +---ContractDtos
|   +---Controllers
|   |   |   DashboardController.cs
|   |   |   KdsController.cs
|   |   |   OrderController.cs
|   |   |   OrderDetailController.cs
|   |   |   PaymentController.cs
|   |   |   TaxController.cs
|   |   |   WaiterController.cs
|   |   |   
|   |   \---Contract
|   |           CatalogContractController.cs
|   |           DashboardContractController.cs
|   |           KdsContractController.cs
|   |           PaymentMethodsContractController.cs
|   |           TaxConfigurationContractController.cs
|   |           TicketPaymentsContractController.cs
|   |           TicketsContractController.cs
|   |           WaitersContractController.cs                  
\---Sales.API
    |   appsettings.Development.json
    |   appsettings.json
    |   Dockerfile
    |   GlobalExceptionHandlerExtensions.cs
    |   Program.cs
    |   Sales.API.csproj
    |   Sales.API.csproj.lscache
    |   Sales.API.http
```

Explicá en 2-3 líneas por qué la organizaste así.

Organicé la estructura de esta manera debido a que estoy implementando Clean Architecture. Donde separo las responsabilidades de mi sistema en capas bien definidas donde se manejan datos, entidades de negocio, lógica de negocio y casos de uso, y presentación de datos. Esto se hace por el motivo de crear un software mantenible y escalable que ayude a reducir las dependencias con software o infraestructura externa en caso de tener que cambiarlo y poder testear mejor la lógica de negocio.

### 2.2 Flujo de "registrar una venta"

Pegá los snippets del código que se ejecuta cuando un usuario confirma una venta, en orden:

1. El endpoint que recibe el request (Controller).

```cs
[EndpointSummary("Procesa el pago de un ticket")]
    [EndpointDescription("""
                         Registra el pago de un ticket usando el metodo indicado.
                         Usar cuando el cliente finaliza la compra.
                         """)]
    [ProducesResponseType(typeof(PayTicketContractResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProcessRestaurantOrderPaymentResultDto), StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> PayTicket(
        string companyCen,
        string ticketCen,
        [FromBody] PayTicketContractRequest request)
    {
        SalesCenLookup? ticket = await salesCenResolver.ResolveTicketAsync(companyCen, ticketCen);
        if (ticket is null)
        {
            return NotFound(new { message = "Ticket no encontrado" });
        }

        int? paymentTypeId = await paymentResolver.ResolvePaymentIdByCode(request.PaymentMethodCode);

        if (paymentTypeId is null)
        {
            return BadRequest(new { message = "Metodo de pago no valido" });
        }

        ProcessRestaurantOrderPaymentResultDto result = await processRestaurantOrderPaymentUseCase.ExecuteAsync(
            new ProcessRestaurantOrderPaymentDto
            {
                RestaurantOrderId = ticket.Id,
                PaymentTypeId = paymentTypeId.Value
            });

        if (!result.IsSuccess)
        {
            return Conflict(result);
        }

        return Ok(new PayTicketContractResponse
        {
            SaleCen = result.SaleCen ?? string.Empty,
            TicketCen = ticket.Cen,
            Status = PaymentStatus.Paid,
            Subtotal = result.Subtotal,
            TaxAmount = result.TaxAmount,
            Total = result.Total,
            InventoryDocumentCen = result.InventoryDocumentCen
        });
    }
```


2. La capa intermedia que procesa la lógica (Service / Use Case / Handler).
```csharp
public async Task<ProcessRestaurantOrderPaymentResultDto> ExecuteAsync(ProcessRestaurantOrderPaymentDto processRestaurantOrderPaymentDto)
    {
        var restaurantOrder = await restaurantOrderRepository.GetByIdAsync(processRestaurantOrderPaymentDto.RestaurantOrderId)
            ?? throw new KeyNotFoundException("La orden del restaurante no existe");

        if (restaurantOrder.Order.Status != OrderStatus.Open)
        {
            throw new InvalidOperationException("La orden no esta abierta para cobro");
        }

        if (restaurantOrder.WaiterId is null)
        {
            throw new InvalidOperationException("No se puede procesar el pago sin mesero asignado");
        }

        if (!await paymentTypeRepository.ExistsAsync(processRestaurantOrderPaymentDto.PaymentTypeId))
        {
            throw new InvalidOperationException("El metodo de pago no es valido");
        }

        string? companyCen = Normalize(restaurantOrder.CompanyCen);
        string? warehouseCen = Normalize(await warehouseConfigurationRepository
            .GetWarehouseCenByCompanyIdAsync(restaurantOrder.CompanyId));

        var orderDetails = await restaurantOrderDetailRepository.GetByRestaurantOrderIdAsync(processRestaurantOrderPaymentDto.RestaurantOrderId);
        var chargeableDetails = orderDetails
            .Where(detail => detail.Status != OrderDetailStatus.Canceled)
            .ToList();

        if (chargeableDetails.Count == 0)
        {
            throw new InvalidOperationException("No existen items cobrables en la orden");
        }

        string? inventoryDocumentCen = null;

        if (!CanUseCenInventory(companyCen, warehouseCen, chargeableDetails))
        {
            throw new InvalidOperationException("No hay datos CEN suficientes para procesar el pago");
        }

        var stockValidationResult = await inventoryService.ValidateStockAsync(
            companyCen!,
            CreateStockValidationRequest(warehouseCen!, restaurantOrder.Cen, chargeableDetails));

        if (!stockValidationResult.IsValid)
        {
            return ProcessRestaurantOrderPaymentResultDto.StockFailure(
                MapContractInsufficiencies(stockValidationResult.Requirements));
        }

        var stockConsumeResult = await inventoryService.ConsumeStockAsync(
            companyCen!,
            CreateStockConsumeRequest(warehouseCen!, restaurantOrder.Cen, chargeableDetails));

        if (!stockConsumeResult.Success)
        {
            return ProcessRestaurantOrderPaymentResultDto.StockFailure(
                MapContractInsufficiencies(stockConsumeResult.Requirements));
        }

        inventoryDocumentCen = stockConsumeResult.DocumentCen;
        var productPriceByCen = await GetProductPricesByCenFromContractAsync(companyCen!, chargeableDetails);

        var saleDetails = new List<SaleDetail>();
        foreach (var detail in chargeableDetails)
        {
            string productCen = detail.ProductCen!;
            if (!productPriceByCen.TryGetValue(productCen, out var price))
            {
                throw new InvalidOperationException($"No se pudo obtener el precio del producto {productCen}");
            }

            saleDetails.Add(new SaleDetail
            {
                ProductId = 0,
                ProductCen = detail.ProductCen,
                Price = price,
                Quantity = detail.Quantity
            });
        }

        var subtotalPrice = saleDetails.Sum(detail => detail.Price * detail.Quantity);

        var sale = new Sale
        {
            SubtotalPrice = subtotalPrice,
            TaxPrice = restaurantOrder.Order.TaxPrice,
            DiscountPercentage = 0,
            SaleDatetime = DateTime.UtcNow,
            CustomerId = restaurantOrder.CustomerId,
            PaymentTypeId = processRestaurantOrderPaymentDto.PaymentTypeId,
            CompanyId = restaurantOrder.CompanyId,
            CompanyCen = restaurantOrder.CompanyCen,
            SaleDetails = saleDetails
        };

        var createdSale = await paymentProcessRepository.CreateSaleAndCloseOrderAsync(
            processRestaurantOrderPaymentDto.RestaurantOrderId,
            sale);

        return ProcessRestaurantOrderPaymentResultDto.Success(
            createdSale.Id,
            createdSale.Cen,
            subtotalPrice,
            restaurantOrder.Order.TaxPrice,
            inventoryDocumentCen);
    }



```

3. La parte que llama al Inventario del compañero (HttpClient o equivalente).

```csharp
public async Task<StockValidationContractResponse> ValidateStockAsync(
        string companyCen,
        StockValidationContractRequest request)
    {
        var response = await http.PostAsJsonAsync(
            $"/api/inventory/companies/{Encode(companyCen)}/stock/validate",
            request);

        response.EnsureSuccessStatusCode();
        return await ReadRequiredAsync<StockValidationContractResponse>(response, "Invalid stock validation contract response");
    }
```

```csharp
public async Task<StockConsumeContractResponse> ConsumeStockAsync(
        string companyCen,
        StockConsumeContractRequest request)
    {
        var response = await http.PostAsJsonAsync(
            $"/api/inventory/companies/{Encode(companyCen)}/stock/consume",
            request);

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            var error = await response.Content.ReadFromJsonAsync<InventoryContractErrorResponse<StockConsumeContractResponse>>();
            return error?.Data ?? new StockConsumeContractResponse
            {
                Success = false
            };
        }

        if (!response.IsSuccessStatusCode)
        {
            string message = await ReadErrorMessageAsync<StockConsumeContractResponse>(response);
            throw new InvalidOperationException(message);
        }

        return await ReadRequiredAsync<StockConsumeContractResponse>(response, "Invalid stock consume contract response");
    }
```

4. La parte que persiste la venta en tu BD.
```csharp
public async Task<SalesCenLookup> CreateSaleAndCloseOrderAsync(int restaurantOrderId, Sale sale)
    {
        await using var transaction = await salesDbContext.Database.BeginTransactionAsync();

        try
        {
            var restaurantOrder = await Queryable
                .Where<RestaurantOrderModel>(salesDbContext.RestaurantOrders
                    .Include(ro => ro.Order), ro => ro.Id == restaurantOrderId && !ro.IsDeleted && !ro.Order.IsDeleted)
                .FirstOrDefaultAsync() ?? throw new KeyNotFoundException("La orden del restaurante no existe");

            var saleModel = new SaleModel
            {
                Cen = string.IsNullOrWhiteSpace(sale.Cen) ? Guid.NewGuid().ToString() : sale.Cen,
                SubtotalPrice = sale.SubtotalPrice,
                TaxPrice = sale.TaxPrice,
                DiscountPercentage = sale.DiscountPercentage,
                SaleDatetime = sale.SaleDatetime,
                CustomerId = sale.CustomerId,
                PaymentTypeId = sale.PaymentTypeId,
                CompanyId = sale.CompanyId,
                CompanyCen = sale.CompanyCen ?? string.Empty,
                SaleDetails = sale.SaleDetails.Select(detail => new SaleDetailModel
                {
                    ProductId = detail.ProductId,
                    ProductCen = detail.ProductCen ?? string.Empty,
                    Price = detail.Price,
                    Quantity = detail.Quantity
                }).ToList()
            };

            await salesDbContext.Sales.AddAsync(saleModel);
            restaurantOrder.Order.OrderStatusId = (int)OrderStatus.Paid;

            await salesDbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new SalesCenLookup(saleModel.Id, saleModel.Cen);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
```

Explicá en 3-5 líneas por qué dividiste así las responsabilidades.

Separé las responsabilidades de esta manera debido a que estoy separando la lógic de negocio y la responsablidad de la persistencia de la forma de presentar y recibir los datos. Tal que si tuviera que cambiar de base de datos o la forma de persistir los datos, o si tuviera que cambiar las reglas de negocio en mi caso de uso, estos cambios no afecten drásticamente a mi lógica core u otra de las capas mas importantes.


### 2.3 Llamada al Inventario del compañero

Pegá el código exacto donde tu Ventas llama al API del Inventario del compañero.

```csharp
var stockValidationResult = await inventoryService.ValidateStockAsync(
            companyCen!,
            CreateStockValidationRequest(warehouseCen!, restaurantOrder.Cen, chargeableDetails));

        if (!stockValidationResult.IsValid)
        {
            return ProcessRestaurantOrderPaymentResultDto.StockFailure(
                MapContractInsufficiencies(stockValidationResult.Requirements));
        }

        var stockConsumeResult = await inventoryService.ConsumeStockAsync(
            companyCen!,
            CreateStockConsumeRequest(warehouseCen!, restaurantOrder.Cen, chargeableDetails));

        if (!stockConsumeResult.Success)
        {
            return ProcessRestaurantOrderPaymentResultDto.StockFailure(
                MapContractInsufficiencies(stockConsumeResult.Requirements));
        }
```

```csharp
public async Task<StockValidationContractResponse> ValidateStockAsync(
        string companyCen,
        StockValidationContractRequest request)
    {
        var response = await http.PostAsJsonAsync(
            $"/api/inventory/companies/{Encode(companyCen)}/stock/validate",
            request);

        response.EnsureSuccessStatusCode();
        return await ReadRequiredAsync<StockValidationContractResponse>(response, "Invalid stock validation contract response");
    }
```

```csharp
public async Task<StockConsumeContractResponse> ConsumeStockAsync(
        string companyCen,
        StockConsumeContractRequest request)
    {
        var response = await http.PostAsJsonAsync(
            $"/api/inventory/companies/{Encode(companyCen)}/stock/consume",
            request);

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            var error = await response.Content.ReadFromJsonAsync<InventoryContractErrorResponse<StockConsumeContractResponse>>();
            return error?.Data ?? new StockConsumeContractResponse
            {
                Success = false
            };
        }

        if (!response.IsSuccessStatusCode)
        {
            string message = await ReadErrorMessageAsync<StockConsumeContractResponse>(response);
            throw new InvalidOperationException(message);
        }

        return await ReadRequiredAsync<StockConsumeContractResponse>(response, "Invalid stock consume contract response");
    }
```

Respondé brevemente:
- ¿Qué pasa si el compañero responde con código 200 OK?
  
En el caso de que mi compañero retorne un status 200, la instrucción del return se ejecutará y la respuesta en json de mi inventario se parseará y se enviará al flujo correspodiente, donde se usará la información retornada para enriquecer los datos para la información de la venta.

- ¿Qué pasa si responde con 404 o 500?

El proceso de validación y de consumo del stock se maneja justo antes de la creación de la venta, por lo que si el backend retorna un status code diferente al esperado de 200, esta línea se ejecutará

```csharp
response.EnsureSuccessStatusCode();
```

Y lanzará una excepción que luego se va a atrapar con el GlobalExceptionHandler implementado en el proyecto de ventas para luego retornar un mensaje no técnico al cliente.

Esto da a entender que si ocurre un excepción el flujo del procesamiento de pagos, la creación de venta nunca se hará, por lo que la consistencia de los datos quedará intacta.

- ¿Qué pasa si el compañero está caído (timeout)?

En caso de que el inventario estuviera caido, en el cliente HTTP encargado de realizar la solicitud a inventario, este retornaría una excepción diferente a la lanzada cuando el status code era diferente a 200. Por lo que es una excepción que directamente será atrapada por el GlobalException handler y se retornará un mensaje de error genérico al usuario.

Se ha decido no realizar un flujo de reintentos automáticos para este caso de uso ya que es un flujo muy importante para el negocio y que modifica la consistencia de los datos de los dos módulos en acción. Por lo que el usuario tendrá que manualmente ejecutar la acción de negocio para realizarla correctamente si es que el módulo de inventario vuelve a estar online.

### 2.4 Configuración de la URL del compañero

Pegá:
- La línea relevante de tu `.env.example` o `appsettings.json`.

**appsettings.Development.json**
```json
"Modules": {
    "InventoryBaseUrl": "http://localhost:5143"
  }
```
Esto es con el motivo de dejar limpio el archivo **appsettings.json** para entornos de producción.

- El código que lee esa configuración y la usa para construir la llamada HTTP.

```csharp
services.AddHttpClient<IInventoryService, InventoryHttpClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["Modules:InventoryBaseUrl"] ??
                                         throw new Exception("InventoryBaseUrl not found"));
        });
```

Esta parte del código es el encargado de leer la configuración de la url base de inventario. Lo que elimina los casos de urls harcodeadas que solo empeoran la legilibilidad y manteniblidad del código.

Explicá en 1 línea cómo cambiarías esa URL si el sábado tu pareja levanta su backend en otra IP.

Si hubiera ese caso, entonces lo que haría es directamente ir al archivo `appsettings.Development.json` para cambiarla configuración de la URL, sin necesidad de revisar otras partes de código

## Sección 3 — Sobre el trabajo en grupo del contrato API

- **3.1** ¿Hubo desacuerdos al definir el contrato? ¿Cuáles?
  - Si hubo algunos desacuerdos ya que todos teníamos nuestra propia lógica de negocio y existían endpoints para casos particulares dependiendo de la persona que los había implementado. El módulo donde mas se tuvo problemas era el de inventario. Endpoints con paso de parámetros por header, diferente forma de nombrar a los recursos y los diferentes esquemas de datos fueron los problemas mas relevantes.
- **3.2** ¿Cómo se resolvieron?
  - Compartimos todos los archivos de open-api y usamos la IA para que genere un contrato que respete las reglas establecidas y ponga endpoints para que cada uno tenga que hacer el mínimo de los cambios.
- **3.3** ¿Qué propusiste vos específicamente que quedó en el contrato final?
  - Propuse el nombre de los recursos de los endpoints del módulo de Ventas y un formato estandarizado para el manejo de errores no esperados (Excepciones) mediante el esquema de datos ProblemDetails. 

## Sección 4 — Teoría aplicada

Respondé cada pregunta en 1-2 párrafos. Está permitido usar IA para mejorar redacción, pero las respuestas deben hacer referencia explícita a tu propio código o decisiones.

**4.1** Tu compañero te avisa que va a cambiar el campo `cantidad` por `qty` en su respuesta del endpoint de stock. Tu sistema ya consume ese endpoint. Explicá qué riesgos genera ese cambio y qué prácticas conocés para evitar que un cambio así rompa los sistemas que dependen de su API.

 Primeramente entre mi compañero y yo se debe definir un contrato que define los recursos públicos y el esquema de datos de entrada y salida del sistema. De esta manera anticipamos y mitigamos los errores de cambio de campos de manera repentina. Por el lado del backend y del frontend, se pueden realizar validaciones del esquema por seguridad, sin embargo, esto solo lo haría para datos o propiedades esenciales, debido a que estaríamos haciendo mucho boiler plate code.

**4.2** Tu sistema de Ventas hace una petición al Inventario para descontar stock. La red se cae justo después de que Inventario procesó el descuento pero antes de que la respuesta llegue a Ventas. ¿Qué problema se genera? ¿Cómo lo manejarías?

El problema genera una inconsistencia de datos, ya que se estaría descontando el stock de manera normal, sin embargo, la venta nunca existiría, por lo que terminaríamos con menor stock disponible sin tener algún registro de las ventas. 

Una solución no tan robusta pero que es igualmente efectiva en la mayoría de los casos, es manejar un rollback controlado en la comunicación entre sistemas. Es decir que si desde ventas no obtenemos una respuesta OK o válida para el sistema, entonces ejecutemos un bloque de código para descontar el stock del sistema de inventario mediante la llamada a un endpoint de devolución. Obviamente en estos casos tenemos que ser bastante cautelosos ya que podríamos estar confundiendo errores de la caída de inventario o de la caída de red directamente, por lo que podríamos causar una inconsistencia por accidente.

Otro enfoque que podríamos utilizar es el uso de una comunicación asíncrona entre sistemas, es decir de alguna manera guardar esos eventos fallidos mediante una tabla o una cola que guarden los eventos importantes para poder realizar un rollback mediante algún tipo de script o proceso que se encargue de leer ese log o eventos fallidos y puedan realizar un rollback en el sistema correspondiente.

**4.3** Si el Inventario del compañero está caído, ¿debería tu Ventas permitir seguir registrando ventas? Justificá considerando ventajas y desventajas de cada postura. ¿Qué hace TU sistema hoy en ese caso?

Tener la posibilidad de registrar ventas sin depender del inventario puede ser muy conveniente en casos donde el sistema de inventario se cae y no podemos permitir que nuestro flujo de negocio se detenga. Sin embargo, esto igualmente podría dejar inconsistencia entre los datos si es que no tenemos un flujo adecuado, aunque también depende del contexto del negocio y como manejan de manera teórica esos casos especiales. 

En este caso lo que podría hacer sería mantener un tipo de estado intermedio entre una venta realizada completamente o por pagar, y que solamente cuando se pueda acceder y verificar el contenido de inventario, se pueda proceder a registrar la venta como completada.

Por otro lado, el dejar de registrar ventas cuando el inventario está caído también es beneficioso ya que evita que se registren ventas nuevas que en el futuro podrían no hacerse debido a la variabilidad y el funcionamiento del sistema en el día. Tendríamos solo los registros que verdaderamente se realizaron y no tendríamos el problema de datos basura que dificulten los análisis futuros. Sin embargo, si el inventario cae, entonces la mayoría del sistema deja de funcionar (por lo menos el flujo más importante del módulo de venta), por lo que inventario se convertiría en el Single Point of Failure de nuestro sistema.

En día de hoy, mi sistema no deja realizar ventas si es que el módulo de inventario se cae, debido a que justo antes de confirmar la venta, se valida la cantidad del stock disponible, y si todo está correcto, entonces recién se prosigue con la venta. Esto me permite asegurar una mejor consistencia entre los datos de los dós módulos, aunque pierdo disponibilidad debido al Single Point of Failure de inventario.

**4.4** Explicá por qué tener la URL del compañero hardcodeada como `http://localhost:5000` es un problema. ¿Cuál es la solución correcta y cómo la implementaste vos?

El problema de tener una URL hardcodeada, es que cuando ocurra el caso de nuestro compañero cambie su IP por algún motivo y tengamos la aplicación desplegada, vamos a tener que cambiar obligatóriamente el código fuente de nuestros módulos e incluso tener que hacer un re-build completo de nuestra aplicación, lo que trae problemas de disponibilidad en nuestro sistema que no son aceptables en un entorno de producción.

La solución correcta es manejarlo mediante variables de entorno que se inyecten en runtime y no en build-time. Esto nos permite cambiar las configuraciones de entorno sin necesidad de dar de baja nuestra aplicación y además centralizamos en un archivo todas las configuraciones necesarias para ejecutarla, por lo cual nuevos desarrolladores no tendrán la necesidad de leer todo el código para ejecutar al aplicación.

Mi solución hace uso de la configuración del entorno backend mediante **appsettings.Development.json** y el en frontend mediante un archivo **.env**. 

Estas variables se inyectan en runtime (Por lo menos en la parte del backend) y no entran en el proceso de compilación / construcción del código, por lo que fácilmente puedo cambiar mi configuración de mi entorno sin tener que volver a hacer un build.
