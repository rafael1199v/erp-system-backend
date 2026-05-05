using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Sales.Infrastructure.Models;

[Table("payment_types")]
public class PaymentTypeModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}