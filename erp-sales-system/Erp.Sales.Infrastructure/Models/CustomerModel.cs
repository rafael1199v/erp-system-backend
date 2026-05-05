using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Sales.Infrastructure.Models;

[Table("customers")]
public class CustomerModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int CompanyId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}