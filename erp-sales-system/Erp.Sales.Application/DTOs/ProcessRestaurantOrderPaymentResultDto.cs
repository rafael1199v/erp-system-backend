namespace Erp.Sales.Application.DTOs;
public class ProcessRestaurantOrderPaymentResultDto
{
    public bool IsSuccess { get; init; }
    public int? SaleId { get; init; }
    public string? InventoryDocumentCen { get; init; }
    public string Message { get; init; } = string.Empty;
    public List<StockInsufficiencyResponseDto> Insufficiencies { get; init; } = new();

    public static ProcessRestaurantOrderPaymentResultDto Success(int saleId, string? inventoryDocumentCen = null)
    {
        return new ProcessRestaurantOrderPaymentResultDto
        {
            IsSuccess = true,
            SaleId = saleId,
            InventoryDocumentCen = inventoryDocumentCen,
            Message = "Pago procesado correctamente"
        };
    }

    public static ProcessRestaurantOrderPaymentResultDto StockFailure(List<StockInsufficiencyResponseDto> insufficiencies)
    {
        return new ProcessRestaurantOrderPaymentResultDto
        {
            IsSuccess = false,
            Message = "Stock insuficiente para completar el pago",
            Insufficiencies = insufficiencies
        };
    }
}
