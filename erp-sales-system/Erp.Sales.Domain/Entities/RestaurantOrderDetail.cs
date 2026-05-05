using Erp.Sales.Domain.Enums;

namespace Erp.Sales.Domain.Entities;

public class RestaurantOrderDetail
{
    public required int Id { get; set; }
    public required int RestaurantOrderId { get; set; }
    public required int ProductId { get; set; }
    public required OrderDetailStatus Status { get; set; }
    public string? Note { get; set; }
    public required int Quantity { get; set; }
    public required int ResendCount { get; set; }
    public DateTime? SentAt { get; set; }
    public required DateTime CreatedAt { get; set; }

    public static RestaurantOrderDetail Create(int restaurantOrderId, int productId, int quantity, string? note)
    {
        if (quantity < 1)
        {
            throw new ArgumentException("Quantity must be greater than or equal to 1.");
        }

        return new RestaurantOrderDetail
        {
            Id = 0,
            RestaurantOrderId = restaurantOrderId,
            ProductId = productId,
            Status = OrderDetailStatus.Created,
            Note = note,
            Quantity = quantity,
            SentAt = null,
            CreatedAt = DateTime.UtcNow,
            ResendCount = 0
        };
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity < 1)
        {
            throw new ArgumentException("Quantity must be greater than or equal to 1.");
        }

        Quantity = quantity;
    }

    public void UpdateNote(string? note) {
        Note = note;
    }

    public void SendDetail()
    {
        if (SentAt == null)
        {
            SentAt = DateTime.UtcNow;
        }
    }
}