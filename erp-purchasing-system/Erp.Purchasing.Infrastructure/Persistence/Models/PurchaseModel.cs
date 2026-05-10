using System.ComponentModel.DataAnnotations.Schema;
using Erp.Purchasing.Domain.Enums;
using Erp.Purchasing.Infrastructure.Persistence.Models.Common;

namespace Erp.Purchasing.Infrastructure.Persistence.Models;

[Table("purchases")]
public class PurchaseModel : AuditableModel
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("cen")]
    public Guid Cen { get; set; } = Guid.NewGuid();
    
    [Column("purchase_status")]
    public PurchaseStatus PurchaseStatus { get; set; }
    
    [Column("supplier_id")]
    public int SupplierId { get; set; }
    public SupplierModel Supplier { get; set; } = null!;
    
    [Column("purchase_date")]
    public DateOnly PurchaseDate { get; set; }
    
    [Column("company_cen")]
    public Guid CompanyCen { get; set; }
    
    public ICollection<PurchaseItemModel> PurchaseItems { get; set; } = new List<PurchaseItemModel>();
}