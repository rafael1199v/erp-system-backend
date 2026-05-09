using Erp.Sales.Application.DTOs;
using Erp.Sales.Application.Interfaces;
using Erp.Sales.Application.UseCases.TaxConfiguration;

namespace Erp.Sales.Application.UseCases.RestaurantOrder;

public class CreateRestaurantOrderUseCase(IRestaurantOrderRepository restaurantOrderRepository, IGetGlobalTaxUseCase globalTaxUseCase) : ICreateRestaurantOrderUseCase
{
    public async Task<int> ExecuteAsync(CreateRestaurantOrderDto createRestaurantOrderDto)
    {
        var globalTax = await globalTaxUseCase.ExecuteAsync(createRestaurantOrderDto.CompanyId);

        var restaurantOrder = Domain.Entities.RestaurantOrder.Create(
            taxPrice: globalTax,
            companyId: createRestaurantOrderDto.CompanyId,
            companyCen: createRestaurantOrderDto.CompanyCen
        );
        
       return await restaurantOrderRepository.CreateAsync(restaurantOrder);
    }
}
