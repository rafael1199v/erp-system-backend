namespace Erp.Sales.Domain.Entities;

public class RestaurantOrder
{
    public required int Id { get; init; }
    public string Cen { get; init; } = string.Empty;
    
    public required int OrderId { get; set; }
    public required Order Order { get; set; }
    
    public required decimal TaxPrice { get; set; }
    
    public required int CompanyId { get; set; }
    public string? CompanyCen { get; set; }
    
    public required DateTime OrderDatetime { get; set; }
    
    public int? WaiterId { get; set; }
    
    public string? WaiterCen { get; set; }

    public int? CustomerId { get; set; }
    
    
    public static RestaurantOrder Create(decimal taxPrice, int companyId, string? companyCen = null)
    {
        return new RestaurantOrder
        {
            TaxPrice = taxPrice,
            CompanyId = companyId,
            CompanyCen = companyCen,
            Id = 0,
            Cen = string.Empty,
            OrderId = 0,
            OrderDatetime = DateTime.UtcNow,
            Order = new Order
            {
                Id = 0,
                CompanyId = companyId,
                OrderDateTime = DateTime.UtcNow,
                DailyNumber = 0,
                TaxPrice = taxPrice
            }
        };
    }
    
}
