using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Application.UseCases.Category;
using Erp.Inventory.Application.UseCases.Company;
using Erp.Inventory.Application.UseCases.Unit;
using Erp.Inventory.Application.UseCases.Warehouse;
using Erp.Inventory.Presentation.ContractDtos;

namespace Erp.Inventory.Presentation.ContractAdapters;

public class InventoryCatalogContractAdapter(
    IInventoryCenResolver cenResolver,
    IInventoryContractMapper mapper,
    IGetCompaniesUseCase getCompaniesUseCase,
    IGetCategoriesByCompanyUseCase getCategoriesByCompanyUseCase,
    ICreateCategoryUseCase createCategoryUseCase,
    IUpdateCategoryUseCase updateCategoryUseCase,
    IGetUnitsByCompanyUseCase getUnitsByCompanyUseCase,
    ICreateUnitUseCase createUnitUseCase,
    IUpdateUnitUseCase updateUnitUseCase,
    IGetWarehousesUseCase getWarehousesUseCase) : IInventoryCatalogContractAdapter
{
    public async Task<List<CompanyContractDto>> GetCompaniesAsync()
    {
        List<GetCompanyDTO> companies = await getCompaniesUseCase.ExecuteAsync();
        return companies.Select(mapper.ToCompanyContract).ToList();
    }

    public async Task<InventoryContractResult<List<CategoryContractDto>>> GetCategoriesAsync(string companyCen)
    {
        CenLookup? company = await cenResolver.ResolveCompanyAsync(companyCen);
        if (company is null)
        {
            return InventoryContractResult<List<CategoryContractDto>>.NotFound("Empresa no encontrada");
        }

        List<CategoryDto> categories = await getCategoriesByCompanyUseCase.ExecuteAsync(company.Id);
        return InventoryContractResult<List<CategoryContractDto>>.Ok(categories.Select(mapper.ToCategoryContract).ToList());
    }

    public async Task<InventoryContractResult<CategoryContractDto>> CreateCategoryAsync(string companyCen, CreateCategoryContractRequest request)
    {
        CenLookup? company = await cenResolver.ResolveCompanyAsync(companyCen);
        if (company is null)
        {
            return InventoryContractResult<CategoryContractDto>.NotFound("Empresa no encontrada");
        }

        int categoryId = await createCategoryUseCase.ExecuteAsync(new CreateCategoryDto(
            Name: request.Name,
            CompanyId: company.Id,
            Description: request.Description));

        CategoryDto createdCategory = (await getCategoriesByCompanyUseCase.ExecuteAsync(company.Id))
            .First(category => category.Id == categoryId);

        return InventoryContractResult<CategoryContractDto>.Ok(mapper.ToCategoryContract(createdCategory));
    }

    public async Task<InventoryContractResult<CategoryContractDto>> UpdateCategoryAsync(string companyCen, string categoryCen, CreateCategoryContractRequest request)
    {
        CenLookup? company = await cenResolver.ResolveCompanyAsync(companyCen);
        if (company is null)
        {
            return InventoryContractResult<CategoryContractDto>.NotFound("Empresa no encontrada");
        }

        CenLookup? category = await cenResolver.ResolveCategoryAsync(company.Id, categoryCen);
        if (category is null)
        {
            return InventoryContractResult<CategoryContractDto>.NotFound("Categoria no encontrada");
        }

        await updateCategoryUseCase.ExecuteAsync(new CategoryDto(
            Id: category.Id,
            Name: request.Name,
            CompanyId: company.Id,
            Cen: category.Cen,
            Description: request.Description));

        return InventoryContractResult<CategoryContractDto>.Ok(new CategoryContractDto
        {
            CategoryCen = category.Cen,
            Name = request.Name,
            Description = request.Description,
            IsActive = true
        });
    }

    public async Task<InventoryContractResult<List<UnitContractDto>>> GetUnitsAsync(string companyCen)
    {
        CenLookup? company = await cenResolver.ResolveCompanyAsync(companyCen);
        if (company is null)
        {
            return InventoryContractResult<List<UnitContractDto>>.NotFound("Empresa no encontrada");
        }

        List<UnitDto> units = await getUnitsByCompanyUseCase.ExecuteAsync(company.Id);
        return InventoryContractResult<List<UnitContractDto>>.Ok(units.Select(mapper.ToUnitContract).ToList());
    }

    public async Task<InventoryContractResult<UnitContractDto>> CreateUnitAsync(string companyCen, CreateUnitContractRequest request)
    {
        CenLookup? company = await cenResolver.ResolveCompanyAsync(companyCen);
        if (company is null)
        {
            return InventoryContractResult<UnitContractDto>.NotFound("Empresa no encontrada");
        }

        int unitId = await createUnitUseCase.ExecuteAsync(new CreateUnitDto(
            Name: request.Name,
            CompanyId: company.Id,
            Abbreviation: request.Abbreviation));

        UnitDto createdUnit = (await getUnitsByCompanyUseCase.ExecuteAsync(company.Id))
            .First(unit => unit.Id == unitId);

        return InventoryContractResult<UnitContractDto>.Ok(mapper.ToUnitContract(createdUnit));
    }

    public async Task<InventoryContractResult<UnitContractDto>> UpdateUnitAsync(string companyCen, string unitCen, CreateUnitContractRequest request)
    {
        CenLookup? company = await cenResolver.ResolveCompanyAsync(companyCen);
        if (company is null)
        {
            return InventoryContractResult<UnitContractDto>.NotFound("Empresa no encontrada");
        }

        CenLookup? unit = await cenResolver.ResolveUnitAsync(company.Id, unitCen);
        if (unit is null)
        {
            return InventoryContractResult<UnitContractDto>.NotFound("Unidad no encontrada");
        }

        await updateUnitUseCase.ExecuteAsync(new UnitDto(
            Id: unit.Id,
            Name: request.Name,
            CompanyId: company.Id,
            Cen: unit.Cen,
            Abbreviation: request.Abbreviation));

        return InventoryContractResult<UnitContractDto>.Ok(new UnitContractDto
        {
            UnitCen = unit.Cen,
            Name = request.Name,
            Abbreviation = request.Abbreviation,
            IsActive = true
        });
    }

    public async Task<InventoryContractResult<List<WarehouseContractDto>>> GetWarehousesAsync(string companyCen)
    {
        CenLookup? company = await cenResolver.ResolveCompanyAsync(companyCen);
        if (company is null)
        {
            return InventoryContractResult<List<WarehouseContractDto>>.NotFound("Empresa no encontrada");
        }

        List<WarehouseDTO> warehouses = await getWarehousesUseCase.GetWarehousesByCompany(company.Id);
        return InventoryContractResult<List<WarehouseContractDto>>.Ok(warehouses.Select(mapper.ToWarehouseContract).ToList());
    }
}
