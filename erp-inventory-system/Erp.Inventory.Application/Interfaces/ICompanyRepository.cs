using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.Interfaces;

public interface ICompanyRepository
{
    public Task<List<CompanyEntity>> GetAllAsync();
}