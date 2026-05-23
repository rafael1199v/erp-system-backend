using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Sales.Infrastructure.Models;

[Table("sales")]
public class SaleModel
{
    public int Id { get; set; }
    public string Cen { get; set; } = Guid.NewGuid().ToString();
   
    public decimal SubtotalPrice { get; set; }
    public decimal TaxPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime SaleDatetime { get; set; }
    
    public int? CustomerId { get; set; }
    public CustomerModel? Customer { get; set; }
    
    public int PaymentTypeId { get; set; }
    public PaymentTypeModel PaymentType { get; set; } = null!;
    
    public int CompanyId { get; set; }
    public string CompanyCen { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;

    public ICollection<SaleDetailModel> SaleDetails { get; set; } = new List<SaleDetailModel>();
}
