using Erp.Sales.Application.Interfaces;
using Erp.Sales.Application.Services;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public class PrintRestaurantOrderUseCase(IGetRestaurantOrderDetailsUseCase getRestaurantOrderDetailsUseCase, IPdfService pdfService) : IPrintRestaurantOrderUseCase
{
    public async Task<byte[]> ExecuteAsync(int restaurantOrderId)
    {
        var restaurantOrderDetails =
            await getRestaurantOrderDetailsUseCase.ExecuteAsync(restaurantOrderId);
        
        return pdfService.GenerateRestaurantOrderPdf(restaurantOrderId, restaurantOrderDetails);
    }
}