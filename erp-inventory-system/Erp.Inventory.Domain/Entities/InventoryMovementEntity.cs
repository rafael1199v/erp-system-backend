using Erp.Inventory.Domain.Enums;

namespace Erp.Inventory.Domain.Entities;

public class InventoryMovementEntity
{
    public required int Id { get; set; }
    public required string Title { get; set; } = string.Empty;
    public required DateOnly MovementDate { get; set; }
    public required MovementTypeEnum MovementType { get; set; }
    public required MovementStatusEnum MovementStatus { get; set; }
    public required int CompanyId { get; set; }
    public List<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();

    public void MakeAdjustment()
    {
        this.MovementType = MovementTypeEnum.Adjustment;

        foreach (var transaction in Transactions)
        {
            transaction.ChangeStatus(TransactionTypeEnum.Adjustment);
        }
    }

    public void ApplyOperation()
    {
        this.MovementStatus = MovementStatusEnum.Completed;
    }
}