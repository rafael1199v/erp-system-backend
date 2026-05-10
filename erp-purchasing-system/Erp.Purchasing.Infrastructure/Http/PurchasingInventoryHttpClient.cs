using System.Net.Http.Json;
using Erp.Inventory.Contracts;
using Erp.Purchasing.Application.DTOs;
using Erp.Purchasing.Application.Services;

namespace Erp.Purchasing.Infrastructure.Http;

public class PurchasingInventoryHttpClient(HttpClient http) : IPurchasingInventoryService
{
    public async Task ConfirmStockIncreaseAsync(
        string companyCen,
        string warehouseCen,
        string purchaseOrderCen,
        IReadOnlyCollection<PurchaseOrderDetailItemDto> items,
        CancellationToken ct = default)
    {
        var request = new StockIncreaseContractRequest
        {
            WarehouseCen = warehouseCen,
            Source = "PURCHASES",
            ReferenceCen = purchaseOrderCen,
            Reason = $"Registro de la compra {purchaseOrderCen}",
            Items = items.Select(item => new StockValidationItemContractDto
            {
                ProductCen = item.ProductCen,
                Quantity = item.Quantity
            }).ToList()
        };

        var response = await http.PostAsJsonAsync(
            $"/api/inventory/companies/{Encode(companyCen)}/stock/increase",
            request,
            ct);

        if (!response.IsSuccessStatusCode)
        {
            var message = await ReadErrorMessageAsync(response, ct);
            throw new InvalidOperationException(message);
        }
    }

    private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response, CancellationToken ct)
    {
        var error = await response.Content.ReadFromJsonAsync<InventoryContractErrorResponse<object>>(cancellationToken: ct);
        return string.IsNullOrWhiteSpace(error?.Message)
            ? $"Inventory stock increase request failed with status {(int)response.StatusCode}"
            : error.Message;
    }

    private static string Encode(string value)
    {
        return Uri.EscapeDataString(value.Trim());
    }
}
