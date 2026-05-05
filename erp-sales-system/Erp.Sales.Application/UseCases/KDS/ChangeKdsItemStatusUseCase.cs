using Erp.Sales.Application.Interfaces;
using Erp.Sales.Domain.Enums;

namespace Erp.Sales.Application.UseCases.KDS;

public class ChangeKdsItemStatusUseCase(IKdsRepository kdsRepository) : IChangeKdsItemStatusUseCase
{
    public async Task ExecuteAsync(int restaurantOrderDetailId, int newStatusId)
    {
        if(!Enum.IsDefined(typeof(OrderDetailStatus), newStatusId))
            throw new ArgumentException($"Estado del item de kds invalido");
        
        await kdsRepository.UpdateKdsItemStatusAsync(restaurantOrderDetailId, (OrderDetailStatus)newStatusId);
    }
}