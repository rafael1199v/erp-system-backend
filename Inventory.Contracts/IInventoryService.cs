namespace Erp.Inventory.Contracts;

public interface IInventoryService
{
    Task<bool> IsProductActiveAsync(int productId, int companyId);
    Task<bool> HasAvailableStockAsync(int productId, int requestedQuantity, int companyId, int warehouseId);
    Task<List<RestaurantOrderProductDto>> GetOrderDetailProductsAsync(int companyId, int warehouseId);
    Task<List<RestaurantOrderDetailProductDto>> GetOrderDetailProductsByIdsAsync(List<int> productIds);
    Task<StockValidationResultDto> ValidateStockAvailabilityAsync(List<StockRequirementDto> requirements, int companyId);
    Task ExecutePaymentStockDiscountAsync(CreatePaymentStockDiscountDto createPaymentStockDiscountDto);
}