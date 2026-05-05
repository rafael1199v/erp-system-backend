namespace Erp.Sales.Domain.Entities;

public class RestaurantOrder
{
    public required int Id { get; init; }
    
    public required int OrderId { get; set; }
    public required Order Order { get; set; }
    
    public required decimal TaxPrice { get; set; }
    
    public required int CompanyId { get; set; }
    
    public required DateTime OrderDatetime { get; set; }
    
    public int? WaiterId { get; set; }

    public int? CustomerId { get; set; }
    
    
    public static RestaurantOrder Create(decimal taxPrice, int companyId)
    {
        return new RestaurantOrder
        {
            TaxPrice = taxPrice,
            CompanyId = companyId,
            Id = 0,
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