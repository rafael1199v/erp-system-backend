namespace Erp.Sales.Application.UseCases.KDS;

public interface IChangeKdsItemStatusUseCase
{
    Task ExecuteAsync(int restaurantOrderDetailId, int newStatusId);
}