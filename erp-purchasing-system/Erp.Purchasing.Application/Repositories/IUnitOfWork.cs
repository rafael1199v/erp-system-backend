namespace Erp.Purchasing.Application.Repositories;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}
