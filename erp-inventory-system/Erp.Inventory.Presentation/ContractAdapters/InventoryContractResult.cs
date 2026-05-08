namespace Erp.Inventory.Presentation.ContractAdapters;

public enum InventoryContractResultStatus
{
    Success,
    NotFound,
    Conflict,
    Invalid
}

public class InventoryContractResult<T>
{
    public InventoryContractResultStatus Status { get; init; }
    public T? Value { get; init; }
    public string? Message { get; init; }

    public bool Succeeded => Status == InventoryContractResultStatus.Success;

    public static InventoryContractResult<T> Ok(T value)
    {
        return new InventoryContractResult<T>
        {
            Status = InventoryContractResultStatus.Success,
            Value = value
        };
    }

    public static InventoryContractResult<T> NotFound(string message)
    {
        return new InventoryContractResult<T>
        {
            Status = InventoryContractResultStatus.NotFound,
            Message = message
        };
    }

    public static InventoryContractResult<T> Conflict(T value, string message)
    {
        return new InventoryContractResult<T>
        {
            Status = InventoryContractResultStatus.Conflict,
            Value = value,
            Message = message
        };
    }

    public static InventoryContractResult<T> Invalid(string message)
    {
        return new InventoryContractResult<T>
        {
            Status = InventoryContractResultStatus.Invalid,
            Message = message
        };
    }
}
