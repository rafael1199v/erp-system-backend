using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Erp.Purchasing.Infrastructure.Persistence.Models.Common;
using Microsoft.Build.Framework;

namespace Erp.Purchasing.Infrastructure.Persistence.Models;

[Table("suppliers")]
public class SupplierModel : AuditableModel
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("cen")]
    public Guid Cen { get; set; } = Guid.NewGuid();
    
    [Column("name"), MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Column("company_cen")]
    public Guid CompanyCen { get; set; }
}