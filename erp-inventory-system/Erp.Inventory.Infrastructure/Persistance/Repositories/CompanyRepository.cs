using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Application.Mappers;
using Erp.Inventory.Domain.Entities;
using Erp.Inventory.Infrastructure.Persistance.Context;
using Erp.Inventory.Infrastructure.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace Erp.Inventory.Infrastructure.Persistance.Repositories;

public class CompanyRepository : ICompanyRepository
{
    public readonly AppDbContext _context;

    public CompanyRepository(AppDbContext context)
    {
        this._context = context;
    }
    public async Task<List<CompanyEntity>> GetAllAsync()
    {
        List<Company> companyModels = await _context.Companies.ToListAsync();
        return companyModels.Select(company => new CompanyEntity
        {
            Id = company.Id,
            Cen = company.Cen,
            Name = company.Name,
        }).ToList();
    }
}
