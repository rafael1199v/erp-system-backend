using System.ComponentModel.DataAnnotations.Schema;
using Erp.Purchasing.Infrastructure.Persistence.Models.Common;

namespace Erp.Purchasing.Infrastructure.Persistence.Models;

[Table("purchase_items")]
public class PurchaseItemModel : AuditableModel
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("product_cen")]
    public Guid ProductCen { get; set; }
    
    [Column("quantity")]
    public int Quantity { get; set; }
    
    [Column("purchase_id")]
    public int PurchaseId { get; set; }
    public PurchaseModel Purchase { get; set; } = null!;
}