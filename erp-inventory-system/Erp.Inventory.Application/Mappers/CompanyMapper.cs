using Erp.Inventory.Application.DTOs;
using Erp.Inventory.Domain.Entities;

namespace Erp.Inventory.Application.Mappers;

public class CompanyMapper : ICompanyMapper
{
    public GetCompanyDTO ToDto(CompanyEntity companyEntity)
    {
        return new GetCompanyDTO
        {
            Id = companyEntity.Id,
            Name = companyEntity.Name
        };
    }
}