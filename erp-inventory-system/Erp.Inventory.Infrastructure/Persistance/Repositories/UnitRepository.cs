using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Domain.Entities;
using Erp.Inventory.Infrastructure.Persistance.Context;
using Erp.Inventory.Infrastructure.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Erp.Inventory.Infrastructure.Persistance.Repositories;

public class UnitRepository(AppDbContext context, ILogger<UnitRepository> logger) : IUnitRepository
{
    public async Task<int> CreateAsync(UnitEntity unit)
    {
        try
        {
            var unitModel = ToModel(unit);
            await context.Units.AddAsync(unitModel);

            await context.SaveChangesAsync();

            return unitModel.Id;
        }
        catch(Exception ex)
        {
            throw new Exception("Ha ocurrido un error al crear la unidad", ex);
        }
        
        
    }

    public async Task<List<UnitEntity>> GetAllByCompanyIdAsync(int companyId)
    {
        var units = await Queryable.Where<Unit>(context.Units, u => u.CompanyId == companyId).ToListAsync();
        return Enumerable.ToList(units.Select(ToDomain));
    }

    public async Task UpdateAsync(UnitEntity unit)
    {
        try
        {
            var unitModel = await context.Units.FirstOrDefaultAsync(u => u.Id == unit.Id);

            if (unitModel == null)
            {
                logger.LogDebug("Unit model not found: {Time}", DateTimeOffset.Now);
                return;
            }
            
            unitModel.Name = unit.Name;
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Hubo un error al editar la unidad", ex);
        }
    }

    private static Unit ToModel(UnitEntity unit)
    {
        return new Unit
        {
            Id = unit.Id,
            Name = unit.Name,
            CompanyId = unit.CompanyId,
        };
    }

    private static UnitEntity ToDomain(Unit model)
    {
        return new UnitEntity
        {
            Id = model.Id,
            Name = model.Name,
            CompanyId = model.CompanyId,
        };
    }
}