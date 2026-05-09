namespace Erp.Inventory.Contracts;

public class InventoryContractErrorResponse<T>
{
    public string? Message { get; set; }
    public T? Data { get; set; }
}
