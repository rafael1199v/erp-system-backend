using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Sales.Infrastructure.Models;

[Table("sale_details")]
public class SaleDetailModel
{
    public int Id { get; set; }
    
    public int SaleId { get; set; }
    public SaleModel Sale { get; set; } = null!;
    
    public int ProductId { get; set; }
    public string? ProductCen { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}
