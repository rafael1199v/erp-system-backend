using Erp.Inventory.Contracts;
using Erp.Inventory.Application.ContractAdapters;
using Erp.Inventory.Application.ContractDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Erp.Inventory.Presentation.Controllers.Contract;

[Route("api/inventory/companies/{companyCen}/products")]
public class InventoryProductsContractController(IInventoryProductContractAdapter productAdapter)
    : InventoryContractControllerBase
{
    [EndpointSummary("Lista productos del catalogo")]
    [EndpointDescription("""
                         Devuelve los productos de una empresa con filtros opcionales.
                         Usar para catalogos, mantenimiento y sincronizacion entre servicios.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(List<ProductContractDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        string companyCen,
        [FromQuery] string? search,
        [FromQuery] string? categoryCen,
        [FromQuery] string? status)
    {
        return ToActionResult(await productAdapter.GetProductsAsync(companyCen, search, categoryCen, status));
    }

    [EndpointSummary("Busca productos por CEN dentro de una empresa")]
    [EndpointDescription("""
                         Este endpoint representa una excepción al formato estándar del contrato de inventario.
                         Aunque la operación es de consulta, se define como POST porque requiere un cuerpo JSON
                         con múltiples criterios de búsqueda. La operación no modifica el estado del sistema.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(List<ProductContractDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpPost("lookup")]
    public async Task<IActionResult> LookupProducts(
        string companyCen,
        [FromBody] ProductLookupContractRequest request)
    {
        return ToActionResult(await productAdapter.LookupProductsAsync(companyCen, request));
    }

    [EndpointSummary("Crea un producto")]
    [EndpointDescription("""
                         Registra un nuevo producto en el catalogo de inventario.
                         Usar para incorporar productos antes de movimientos o ventas.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(CreateProductContractResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> CreateProduct(string companyCen, [FromBody] CreateProductContractRequest request)
    {
        return ToCreatedResult(await productAdapter.CreateProductAsync(companyCen, request));
    }

    [EndpointSummary("Actualiza un producto")]
    [EndpointDescription("""
                         Modifica los datos de un producto existente.
                         Usar para actualizar precios, nombres o categoria.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(ProductContractDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpPut("{productCen}")]
    public async Task<IActionResult> UpdateProduct(
        string companyCen,
        string productCen,
        [FromBody] UpdateProductContractRequest request)
    {
        return ToActionResult(await productAdapter.UpdateProductAsync(companyCen, productCen, request));
    }

    [EndpointSummary("Actualiza el estado de un producto")]
    [EndpointDescription("""
                         Cambia el estado de disponibilidad del producto (activo/inactivo).
                         Usar para controlar que productos se ofrezcan en ventas o compras.
                         Forma parte del contrato de integracion entre servicios.
                         """)]
    [ProducesResponseType(typeof(ProductContractDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [HttpPatch("{productCen}/status")]
    public async Task<IActionResult> UpdateProductStatus(
        string companyCen,
        string productCen,
        [FromBody] UpdateProductStatusContractRequest request)
    {
        return ToActionResult(await productAdapter.UpdateProductStatusAsync(companyCen, productCen, request));
    }
}
