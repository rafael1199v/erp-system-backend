namespace Erp.Inventory.Application.ContractAdapters;

internal static class InventoryContractAdapterDefaults
{
    public const int ActiveProductStatusId = 1;
    public const int OutOfStockProductStatusId = 4;
    public const int DefaultSupplierId = 1;

    public const string ActiveStatus = "ACTIVE";
    public const string InactiveStatus = "INACTIVE";
    public const string OutOfStockStatus = "OUT_OF_STOCK";
    public const string RegisteredStatus = "REGISTERED";
    public const string InsufficientStockReason = "INSUFFICIENT_STOCK";

    public static int ToInternalQuantity(decimal quantity, string fieldName)
    {
        if (quantity <= 0)
        {
            throw new InvalidOperationException($"{fieldName} debe ser mayor a cero");
        }

        if (quantity != decimal.Truncate(quantity))
        {
            throw new InvalidOperationException($"{fieldName} debe ser un numero entero");
        }

        return decimal.ToInt32(quantity);
    }
}
