namespace Erp.Sales.Application.DTOs;
public class ProcessRestaurantOrderPaymentResultDto
{
    public bool IsSuccess { get; init; }
    public int? SaleId { get; init; }
    public string? SaleCen { get; init; }
    public string? InventoryDocumentCen { get; init; }
    public decimal Subtotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal Total { get; init; }
    public string Message { get; init; } = string.Empty;
    public List<StockInsufficiencyResponseDto> Insufficiencies { get; init; } = new();

    public static ProcessRestaurantOrderPaymentResultDto Success(
        int saleId,
        string saleCen,
        decimal subtotal,
        decimal taxAmount,
        string? inventoryDocumentCen = null)
    {
        return new ProcessRestaurantOrderPaymentResultDto
        {
            IsSuccess = true,
            SaleId = saleId,
            SaleCen = saleCen,
            InventoryDocumentCen = inventoryDocumentCen,
            Subtotal = subtotal,
            TaxAmount = taxAmount,
            Total = subtotal + taxAmount,
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
