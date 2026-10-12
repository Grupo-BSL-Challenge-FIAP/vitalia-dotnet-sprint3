using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vitalia.Application.DTOs.Product;
using Vitalia.Application.Interfaces.Repositories;
using Vitalia.API.Hateoas;

namespace Vitalia.API.Controllers;

/// <summary>
/// Consulta de produtos armazenados no MongoDB.
/// </summary>
[Route("api/nosql/products")]
[ApiController]
[Produces("application/json")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProductNoSqlController(
    IProductNoSqlRepository productNoSqlRepository,
    ILogger<ProductNoSqlController> logger) : ControllerBase
{
    /// <summary>
    /// Lista todos os produtos armazenados no MongoDB.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<HateoasResource<ProductResponse>>),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized
    )]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status500InternalServerError
    )]
    public async Task<IActionResult> GetAll()
    {
        logger.LogInformation(
            "Buscando todos os produtos armazenados no MongoDB."
        );

        var products = await productNoSqlRepository
            .GetAllAsync();

        var resources = products
            .Select(CreateProductResource);

        return Ok(resources);
    }

    /// <summary>
    /// Busca um produto armazenado no MongoDB pelo ID.
    /// </summary>
    [HttpGet("{productId:int}")]
    [ProducesResponseType(
        typeof(HateoasResource<ProductResponse>),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound
    )]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized
    )]
    public async Task<IActionResult> GetByProductId(int productId)
    {
        logger.LogInformation(
            "Buscando produto {ProductId} no MongoDB.",
            productId
        );

        var product = await productNoSqlRepository
            .GetByProductIdAsync(productId);

        if (product is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Produto não encontrado.",
                Detail =
                    $"Não foi encontrado um produto com o ID {productId} no MongoDB.",
                Status = StatusCodes.Status404NotFound
            });
        }

        var resource = CreateProductResource(product);

        return Ok(resource);
    }

    /// <summary>
    /// Lista os produtos armazenados no MongoDB por status.
    /// </summary>
    [HttpGet("status/{status}")]
    [ProducesResponseType(
        typeof(IEnumerable<HateoasResource<ProductResponse>>),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized
    )]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status500InternalServerError
    )]
    public async Task<IActionResult> GetByStatus(string status)
    {
        logger.LogInformation(
            "Buscando produtos com status {Status} no MongoDB.",
            status
        );

        var products = await productNoSqlRepository
            .GetByStatusAsync(status.ToUpperInvariant());

        var resources = products
            .Select(CreateProductResource);

        return Ok(resources);
    }

    private static HateoasResource<ProductResponse> CreateProductResource(
        ProductResponse product)
    {
        return new HateoasResource<ProductResponse>
        {
            Data = product,
            Links =
            [
                new HateoasLink
                {
                    Rel = "self",
                    Href = $"/api/nosql/products/{product.Id}",
                    Method = "GET"
                },
                new HateoasLink
                {
                    Rel = "all-products",
                    Href = "/api/nosql/products",
                    Method = "GET"
                },
                new HateoasLink
                {
                    Rel = "status",
                    Href = $"/api/nosql/products/status/{product.Status}",
                    Method = "GET"
                }
            ]
        };
    }
}