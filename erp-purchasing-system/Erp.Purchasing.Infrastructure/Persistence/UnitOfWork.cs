using Erp.Purchasing.Application.Repositories;
using Erp.Purchasing.Infrastructure.Persistence.Context;

namespace Erp.Purchasing.Infrastructure.Persistence;

public class UnitOfWork(PurchasingDbContext context) : IUnitOfWork
{
    public Task<int> CommitAsync(CancellationToken ct = default)
    {
        return context.SaveChangesAsync(ct);
    }
}
