namespace Erp.Inventory.Contracts;

public interface IInventoryService
{
    Task<bool> IsProductActiveAsync(int productId, int companyId);
    Task<bool> HasAvailableStockAsync(int productId, int requestedQuantity, int companyId, int warehouseId);
    Task<List<RestaurantOrderProductDto>> GetOrderDetailProductsAsync(int companyId, int warehouseId);
    Task<List<RestaurantOrderDetailProductDto>> GetOrderDetailProductsByIdsAsync(List<int> productIds);
    Task<StockValidationResultDto> ValidateStockAvailabilityAsync(List<StockRequirementDto> requirements, int companyId);
    Task ExecutePaymentStockDiscountAsync(CreatePaymentStockDiscountDto createPaymentStockDiscountDto);
    Task<CompanyLookupContractDto?> GetCompanyByCenAsync(string companyCen)
    {
        throw new NotSupportedException("CEN company contract access is not implemented by this inventory service.");
    }

    Task<List<ProductContractDto>> GetProductsAsync(
        string companyCen,
        string? search = null,
        string? categoryCen = null,
        string? status = null)
    {
        throw new NotSupportedException("CEN product contract access is not implemented by this inventory service.");
    }

    Task<List<StockItemContractDto>> GetStockAsync(
        string companyCen,
        string? productCen = null,
        string? warehouseCen = null)
    {
        throw new NotSupportedException("CEN stock contract access is not implemented by this inventory service.");
    }

    Task<StockValidationContractResponse> ValidateStockAsync(
        string companyCen,
        StockValidationContractRequest request)
    {
        throw new NotSupportedException("CEN stock validation is not implemented by this inventory service.");
    }

    Task<StockConsumeContractResponse> ConsumeStockAsync(
        string companyCen,
        StockConsumeContractRequest request)
    {
        throw new NotSupportedException("CEN stock consume is not implemented by this inventory service.");
    }
}
