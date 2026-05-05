using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Domain.Entities;
using Erp.Inventory.Infrastructure.Persistance.Context;
using Erp.Inventory.Infrastructure.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace Erp.Inventory.Infrastructure.Persistance.Repositories;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public async Task<int> Create(CategoryEntity categoryEntity)
    {
        try
        {
            var categoryModel = ToModel(categoryEntity);
            await context.Categories.AddAsync(categoryModel);
            
            await context.SaveChangesAsync();

            return categoryModel.Id;
        }
        catch (Exception ex)
        {
            throw new Exception("Hubo un error al crear la categoria", ex);
        }
        
    }

    public async Task<List<CategoryEntity>> GetAllByCompany(int companyId)
    {
        try
        {
            var categories = await Queryable
                .Where<Category>(context.Categories, c => c.CompanyId == companyId).ToListAsync();

            return Enumerable.ToList(categories.Select(ToDomain));
        }
        catch (Exception ex)
        {
            throw new Exception("Hubo un error al obtener las categorias", ex);
        }
    }

    public async Task UpdateAsync(CategoryEntity categoryEntity)
    {
        try
        {
            var categoryModel = await context.Categories.FindAsync(categoryEntity.Id);

            if (categoryModel is null)
                throw new Exception("La categoria no existe");

            categoryModel.Name = categoryEntity.Name;
            await context.SaveChangesAsync();
        }
        catch
        {
            throw new Exception("Hubo un error al actualizar la categoria");
        }
    }


    private static Category ToModel(CategoryEntity categoryEntity)
    {
        return new Category
        {
            Id = categoryEntity.Id,
            Cen = string.IsNullOrWhiteSpace(categoryEntity.Cen) ? Guid.NewGuid().ToString() : categoryEntity.Cen,
            Name = categoryEntity.Name,
            Description = categoryEntity.Description,
            CompanyId = categoryEntity.CompanyId 
        };
        
    }

    private static CategoryEntity ToDomain(Category category)
    {
        return new CategoryEntity
        {
            Id = category.Id,
            Cen = category.Cen,
            Name = category.Name,
            Description = category.Description,
            CompanyId = category.CompanyId
        };
    }
}
