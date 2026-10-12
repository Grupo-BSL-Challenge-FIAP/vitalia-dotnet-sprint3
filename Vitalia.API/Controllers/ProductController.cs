using Microsoft.AspNetCore.Mvc;
using Vitalia.Application.DTOs.Product;
using Vitalia.Application.Interfaces.Services;
using Vitalia.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Vitalia.API.Hateoas;

namespace Vitalia.API.Controllers;

/// <summary>
/// Produtos disponíveis para comercialização na Vitalia.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProductController(
    IProductService productService,
    ILogger<ProductController> logger) : ControllerBase
{
    /// <summary>
    /// Lista todos os produtos.
    /// </summary>
    /// <response code="200">Produtos encontrados.</response>
    /// <response code="500">Erro interno do servidor.</response>
    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<HateoasResource<ProductResponse>>),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status500InternalServerError
    )]
    public async Task<IActionResult> GetAll()
    {
        logger.LogInformation("Listando produtos.");

        var products = await productService.GetAllAsync();

        var resources = products
            .Select(CreateProductResource);

        return Ok(resources);
    }

    /// <summary>
    /// Retorna uma lista paginada e ordenada de produtos.
    /// </summary>
    /// <param name="page">
    /// Número da página que será retornada.
    /// </param>
    /// <param name="pageSize">
    /// Quantidade de produtos por página.
    /// </param>
    /// <param name="sortBy">
    /// Campo utilizado para ordenação: id, name, price, stock ou createdAt.
    /// </param>
    /// <param name="sortDirection">
    /// Direção da ordenação: asc ou desc.
    /// </param>
    /// <returns>
    /// Lista paginada contendo os produtos e informações de paginação.
    /// </returns>
    /// <response code="200">
    /// Produtos retornados com sucesso.
    /// </response>
    /// <response code="400">
    /// Os parâmetros informados são inválidos.
    /// </response>
    [HttpGet("paged")]
    [ProducesResponseType(
        typeof(HateoasResource<ProductPagedResponse>),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest
    )]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "id",
        [FromQuery] string sortDirection = "asc")
    {
        if (page < 1)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Página inválida.",
                Detail = "A página deve ser maior que zero.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Tamanho da página inválido.",
                Detail = "O tamanho da página deve estar entre 1 e 100.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var validSortFields = new[]
        {
            "id",
            "name",
            "price",
            "stock",
            "createdAt"
        };

        if (!validSortFields.Contains(
                sortBy,
                StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Campo de ordenação inválido.",
                Detail =
                    "Utilize id, name, price, stock ou createdAt.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (!sortDirection.Equals(
                "asc",
                StringComparison.OrdinalIgnoreCase) &&
            !sortDirection.Equals(
                "desc",
                StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Direção de ordenação inválida.",
                Detail = "Utilize asc ou desc.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        logger.LogInformation(
            "Listando produtos paginados. Página: {Page}, Tamanho: {PageSize}, Ordenação: {SortBy} {SortDirection}.",
            page,
            pageSize,
            sortBy,
            sortDirection
        );

        var products = await productService.GetPagedAsync(
            page,
            pageSize,
            sortBy,
            sortDirection
        );

        var resource = new HateoasResource<ProductPagedResponse>
        {
            Data = products,
            Links =
            [
                new HateoasLink
                {
                    Rel = "self",
                    Href =
                        $"/api/Product/paged?page={page}&pageSize={pageSize}&sortBy={sortBy}&sortDirection={sortDirection}",
                    Method = "GET"
                },
                new HateoasLink
                {
                    Rel = "all-products",
                    Href = "/api/Product",
                    Method = "GET"
                }
            ]
        };

        return Ok(resource);
    }

    /// <summary>
    /// Busca um produto pelo ID.
    /// </summary>
    /// <param name="id">Identificador do produto.</param>
    /// <response code="200">Produto encontrado.</response>
    /// <response code="404">Produto não encontrado.</response>
    /// <response code="500">Erro interno do servidor.</response>
    [HttpGet("{id:long}")]
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
        StatusCodes.Status500InternalServerError
    )]
    public async Task<IActionResult> GetById(long id)
    {
        logger.LogInformation(
            "Buscando produto: {ProductId}",
            id
        );

        var product = await productService.GetByIdAsync(id);

        if (product is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Produto não encontrado.",
                Detail = $"Não foi encontrado um produto com o ID {id}.",
                Status = StatusCodes.Status404NotFound
            });
        }

        var resource = CreateProductResource(product);

        return Ok(resource);
    }

    /// <summary>
    /// Lista os produtos de uma determinada categoria.
    /// </summary>
    /// <param name="categoryId">Identificador da categoria.</param>
    /// <response code="200">Produtos encontrados.</response>
    /// <response code="500">Erro interno do servidor.</response>
    [HttpGet("category/{categoryId:long}")]
    [ProducesResponseType(
        typeof(IEnumerable<HateoasResource<ProductResponse>>),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status500InternalServerError
    )]
    public async Task<IActionResult> GetByCategory(long categoryId)
    {
        logger.LogInformation(
            "Listando produtos da categoria: {CategoryId}",
            categoryId
        );

        var products = await productService
            .GetByCategoryIdAsync(categoryId);

        var resources = products
            .Select(CreateProductResource);

        return Ok(resources);
    }

    /// <summary>
    /// Lista os produtos de acordo com o status.
    /// </summary>
    /// <param name="status">Status do produto.</param>
    /// <response code="200">Produtos encontrados.</response>
    /// <response code="500">Erro interno do servidor.</response>
    [HttpGet("status/{status}")]
    [ProducesResponseType(
        typeof(IEnumerable<HateoasResource<ProductResponse>>),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status500InternalServerError
    )]
    public async Task<IActionResult> GetByStatus(ProductStatus status)
    {
        logger.LogInformation(
            "Listando produtos com status: {Status}",
            status
        );

        var products = await productService
            .GetByStatusAsync(status);

        var resources = products
            .Select(CreateProductResource);

        return Ok(resources);
    }

    /// <summary>
    /// Cadastra um novo produto.
    /// </summary>
    /// <param name="request">Dados do produto.</param>
    /// <response code="201">Produto criado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="500">Erro interno do servidor.</response>
    [HttpPost]
    [ProducesResponseType(
        typeof(ProductResponse),
        StatusCodes.Status201Created
    )]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest
    )]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status500InternalServerError
    )]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create(
        [FromBody] ProductRequest request)
    {
        logger.LogInformation(
            "Iniciando cadastro do produto: {ProductName}",
            request.Name
        );

        var created = await productService.AddAsync(request);

        logger.LogInformation(
            "Produto criado com sucesso: {ProductId}",
            created.Id
        );

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created
        );
    }

    /// <summary>
    /// Atualiza um produto existente.
    /// </summary>
    /// <param name="id">Identificador do produto.</param>
    /// <param name="request">Novos dados do produto.</param>
    /// <response code="204">Produto atualizado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="404">Produto não encontrado.</response>
    /// <response code="500">Erro interno do servidor.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest
    )]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound
    )]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status500InternalServerError
    )]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] ProductRequest request)
    {
        logger.LogInformation(
            "Atualizando produto: {ProductId}",
            id
        );

        await productService.UpdateAsync(id, request);

        return NoContent();
    }

    /// <summary>
    /// Exclui um produto.
    /// </summary>
    /// <param name="id">Identificador do produto.</param>
    /// <response code="204">Produto excluído com sucesso.</response>
    /// <response code="404">Produto não encontrado.</response>
    /// <response code="500">Erro interno do servidor.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound
    )]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status500InternalServerError
    )]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete(long id)
    {
        logger.LogInformation(
            "Excluindo produto: {ProductId}",
            id
        );

        await productService.DeleteAsync(id);

        return NoContent();
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
                    Href = $"/api/Product/{product.Id}",
                    Method = "GET"
                },
                new HateoasLink
                {
                    Rel = "all-products",
                    Href = "/api/Product",
                    Method = "GET"
                },
                new HateoasLink
                {
                    Rel = "update",
                    Href = $"/api/Product/{product.Id}",
                    Method = "PUT"
                },
                new HateoasLink
                {
                    Rel = "delete",
                    Href = $"/api/Product/{product.Id}",
                    Method = "DELETE"
                }
            ]
        };
    }
}