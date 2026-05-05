using System.Net.Http.Json;
using Erp.Inventory.Contracts;

namespace Erp.Sales.Infrastructure.Http;

    public class InventoryHttpClient(HttpClient http) : IInventoryService
    {
        public async Task<bool> IsProductActiveAsync(int productId, int companyId)
        {
            var response = await http.GetAsync($"/api/inventory/product/{productId}/is-active/{companyId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> HasAvailableStockAsync(int productId, int requestedQuantity, int companyId, int warehouseId)
        {
            var response = await http.GetAsync($"/api/inventory/product/{productId}/available-stock/{companyId}/{warehouseId}/{requestedQuantity}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<List<RestaurantOrderProductDto>> GetOrderDetailProductsAsync(int companyId, int warehouseId)
        {
            var response = await http.GetAsync($"/api/inventory/product/kds/{companyId}/{warehouseId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<RestaurantOrderProductDto>>() ?? throw new Exception("Invalid order detail response");
        }

        public async Task<List<RestaurantOrderDetailProductDto>> GetOrderDetailProductsByIdsAsync(List<int> productIds)
        {
            var response = await http.PostAsJsonAsync("/api/inventory/product/kds", productIds);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<RestaurantOrderDetailProductDto>>() ?? throw new Exception("Invalid order detail response");
        }

        public async Task<StockValidationResultDto> ValidateStockAvailabilityAsync(List<StockRequirementDto> requirements, int companyId)
        {
            StockValidationDto stockValidationDto = new StockValidationDto
            {
                Requirements = requirements,
                CompanyId = companyId
            };
            var response = await http.PostAsJsonAsync($"/api/inventory/product/valid-stock", stockValidationDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<StockValidationResultDto>() ?? throw new Exception("Invalid stock validation response");
        }

        public async Task ExecutePaymentStockDiscountAsync(CreatePaymentStockDiscountDto createPaymentStockDiscountDto)
        {
            var response = await http.PostAsJsonAsync($"/api/inventory/movement/payment", createPaymentStockDiscountDto);
            response.EnsureSuccessStatusCode();
        }
    }