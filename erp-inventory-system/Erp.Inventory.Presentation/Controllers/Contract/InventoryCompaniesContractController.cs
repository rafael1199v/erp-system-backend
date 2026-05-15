using Erp.Inventory.Contracts;
using Erp.Inventory.Presentation.ContractAdapters;
using Erp.Inventory.Presentation.ContractDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers.Contract;

[Route("api/inventory/companies")]
public class InventoryCompaniesContractController(IInventoryCatalogContractAdapter catalogAdapter)
    : InventoryContractControllerBase
{
    [EndpointSummary("Lista empresas del contrato de inventario")]
    [EndpointDescription("""
                         Devuelve el catalogo de empresas disponibles en inventario.
                         Usar para descubrir empresas habilitadas antes de consultar catalogos y stock.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(List<CompanyContractDto>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetCompanies()
    {
        return Ok(await catalogAdapter.GetCompaniesAsync());
    }

    [EndpointSummary("Obtiene una empresa por CEN")]
    [EndpointDescription("""
                         Devuelve los datos de una empresa segun su CEN.
                         Usar para validar que el CEN exista antes de operaciones de inventario e integraciones con el modulo de ventas.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(CompanyLookupContractDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpGet("{companyCen}")]
    public async Task<IActionResult> GetCompany(string companyCen)
    {
        return ToActionResult(await catalogAdapter.GetCompanyByCenAsync(companyCen));
    }
}
