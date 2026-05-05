using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Application.Interfaces;
using Erp.Inventory.Application.Mappers;

namespace Erp.Inventory.Application.UseCases.Company;

public class GetCompaniesUseCase : IGetCompaniesUseCase
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ICompanyMapper _companyMapper;
 
    public GetCompaniesUseCase(ICompanyRepository companyRepository, ICompanyMapper companyMapper)
    {
        _companyRepository = companyRepository;
        _companyMapper = companyMapper;
    }
    
    public async Task<List<GetCompanyDTO>> ExecuteAsync()
    {
        var companies = await _companyRepository.GetAllAsync();
        List<GetCompanyDTO> companiesDto = companies.Select(company => _companyMapper.ToDto(company)).ToList();

        return companiesDto;
    }
}