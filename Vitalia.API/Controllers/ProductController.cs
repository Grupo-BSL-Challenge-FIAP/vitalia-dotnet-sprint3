using Microsoft.AspNetCore.Mvc;
using Vitalia.Application.DTOs.Product;
using Vitalia.Application.Interfaces.Services;

namespace Vitalia.API.Controllers;

/// <summary>
/// Produtos disponíveis para comercialização na Vitalia.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
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
    [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        logger.LogInformation("Listando produtos.");

        var products = await productService.GetAllAsync();

        return Ok(products);
    }

    /// <summary>
    /// Busca um produto pelo ID.
    /// </summary>
    /// <param name="id">Identificador do produto.</param>
    /// <response code="200">Produto encontrado.</response>
    /// <response code="404">Produto não encontrado.</response>
    /// <response code="500">Erro interno do servidor.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(long id)
    {
        logger.LogInformation(
            "Buscando produto por ID: {ProductId}",
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

        return Ok(product);
    }

    /// <summary>
    /// Cadastra um novo produto.
    /// </summary>
    /// <param name="request">Dados do produto.</param>
    /// <response code="201">Produto criado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="500">Erro interno do servidor.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] ProductRequest request)
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(long id)
    {
        logger.LogInformation(
            "Excluindo produto: {ProductId}",
            id
        );

        await productService.DeleteAsync(id);

        return NoContent();
    }
}