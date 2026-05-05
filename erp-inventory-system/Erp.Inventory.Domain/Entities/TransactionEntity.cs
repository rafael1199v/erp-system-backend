using Erp.Inventory.Domain.Enums;

namespace Erp.Inventory.Domain.Entities;

public class TransactionEntity
{
    public required int Id { get; set; }
    public string Cen { get; set; } = Guid.NewGuid().ToString();
    public required int Quantity { get; set; }
    public required string Reason { get; set; } = string.Empty;
    public required DateOnly TransactionDate { get; set; }
    public required TransactionTypeEnum TransactionType { get; set; }
    
    //For improving query performances instead of querying the entire object
    public required int WarehouseId { get; set; }
    public required int ProductId { get; set; }
    
    public ProductEntity? Product { get; set; }
    public WarehouseEntity? Warehouse { get; set; }
    
    public void ChangeStatus(TransactionTypeEnum newStatus)
    {
        this.TransactionType = newStatus;  
    }
}
