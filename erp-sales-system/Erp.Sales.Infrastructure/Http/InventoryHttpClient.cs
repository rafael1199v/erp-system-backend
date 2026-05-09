using System.Net;
using System.Net.Http.Json;
using Erp.Inventory.Contracts;

namespace Erp.Sales.Infrastructure.Http;

public class InventoryHttpClient(HttpClient http) : IInventoryService
{
    public async Task<CompanyLookupContractDto?> GetCompanyByCenAsync(string companyCen)
    {
        var response = await http.GetAsync($"/api/inventory/companies/{Encode(companyCen)}");
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await ReadRequiredAsync<CompanyLookupContractDto>(response, "Invalid company lookup contract response");
    }

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
        return await ReadRequiredAsync<List<RestaurantOrderProductDto>>(response, "Invalid order detail response");
    }

    public async Task<List<RestaurantOrderDetailProductDto>> GetOrderDetailProductsByIdsAsync(List<int> productIds)
    {
        var response = await http.PostAsJsonAsync("/api/inventory/product/kds", productIds);
        response.EnsureSuccessStatusCode();
        return await ReadRequiredAsync<List<RestaurantOrderDetailProductDto>>(response, "Invalid order detail response");
    }

    public async Task<StockValidationResultDto> ValidateStockAvailabilityAsync(List<StockRequirementDto> requirements, int companyId)
    {
        StockValidationDto stockValidationDto = new()
        {
            Requirements = requirements,
            CompanyId = companyId
        };

        var response = await http.PostAsJsonAsync("/api/inventory/product/valid-stock", stockValidationDto);
        response.EnsureSuccessStatusCode();
        return await ReadRequiredAsync<StockValidationResultDto>(response, "Invalid stock validation response");
    }

    public async Task ExecutePaymentStockDiscountAsync(CreatePaymentStockDiscountDto createPaymentStockDiscountDto)
    {
        var response = await http.PostAsJsonAsync("/api/inventory/movement/payment", createPaymentStockDiscountDto);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<ProductContractDto>> GetProductsAsync(
        string companyCen,
        string? search = null,
        string? categoryCen = null,
        string? status = null)
    {
        string endpoint = $"/api/inventory/companies/{Encode(companyCen)}/products"
                          + BuildQuery(("search", search), ("categoryCen", categoryCen), ("status", status));

        var response = await http.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return await ReadRequiredAsync<List<ProductContractDto>>(response, "Invalid product contract response");
    }

    public async Task<List<StockItemContractDto>> GetStockAsync(
        string companyCen,
        string? productCen = null,
        string? warehouseCen = null)
    {
        string endpoint = $"/api/inventory/companies/{Encode(companyCen)}/stock"
                          + BuildQuery(("productCen", productCen), ("warehouseCen", warehouseCen));

        var response = await http.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return await ReadRequiredAsync<List<StockItemContractDto>>(response, "Invalid stock contract response");
    }

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

    public async Task<List<SellableProductContractDto>> GetSellableProductsAsync(
        string companyCen,
        string? search = null,
        string? categoryCen = null,
        string? warehouseCen = null,
        bool onlyAvailable = true,
        int page = 1,
        int pageSize = 50)
    {
        string endpoint = $"/api/inventory/companies/{Encode(companyCen)}/sellable-products"
                          + BuildQuery(
                              ("search", search),
                              ("categoryCen", categoryCen),
                              ("warehouseCen", warehouseCen),
                              ("onlyAvailable", onlyAvailable.ToString().ToLowerInvariant()),
                              ("page", page.ToString()),
                              ("pageSize", pageSize.ToString()));

        var response = await http.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return await ReadRequiredAsync<List<SellableProductContractDto>>(response, "Invalid sellable product contract response");
    }

    private static async Task<T> ReadRequiredAsync<T>(HttpResponseMessage response, string errorMessage)
    {
        return await response.Content.ReadFromJsonAsync<T>()
               ?? throw new InvalidOperationException(errorMessage);
    }

    private static async Task<string> ReadErrorMessageAsync<T>(HttpResponseMessage response)
    {
        var error = await response.Content.ReadFromJsonAsync<InventoryContractErrorResponse<T>>();
        return string.IsNullOrWhiteSpace(error?.Message)
            ? $"Inventory request failed with status {(int)response.StatusCode}"
            : error.Message;
    }

    private static string BuildQuery(params (string Name, string? Value)[] parameters)
    {
        var query = parameters
            .Where(parameter => !string.IsNullOrWhiteSpace(parameter.Value))
            .Select(parameter => $"{parameter.Name}={Encode(parameter.Value!)}")
            .ToList();

        return query.Count == 0 ? string.Empty : "?" + string.Join("&", query);
    }

    private static string Encode(string value)
    {
        return Uri.EscapeDataString(value.Trim());
    }
}
