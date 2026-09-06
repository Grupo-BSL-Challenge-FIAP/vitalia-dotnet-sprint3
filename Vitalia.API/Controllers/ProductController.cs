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
    /// Cadastra um novo produto.
    /// </summary>
    /// <param name="request">Dados do produto.</param>
    /// <response code="201">Produto criado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="500">Erro interno do servidor.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] ProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        logger.LogInformation(
            "Iniciando cadastro do produto: {ProductName}",
            request.Name
        );

        await productService.AddAsync(request);

        logger.LogInformation(
            "Produto cadastrado com sucesso: {ProductName}",
            request.Name
        );

        return StatusCode(StatusCodes.Status201Created);
    }
}