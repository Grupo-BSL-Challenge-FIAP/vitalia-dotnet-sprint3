using Microsoft.AspNetCore.Mvc;
using Vitalia.Application.DTOs.Category;
using Vitalia.Application.Interfaces.Services;

namespace Vitalia.API.Controllers;

/// <summary>
/// Gerencia as categorias de produtos da Vitalia.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class CategoryController(
    ICategoryService categoryService,
    ILogger<CategoryController> logger) : ControllerBase
{
    /// <summary>
    /// Retorna todas as categorias cadastradas.
    /// </summary>
    /// <returns>Lista de categorias.</returns>
    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<CategoryResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        logger.LogInformation("Listando categorias.");

        var categories = await categoryService.GetAllAsync();

        return Ok(categories);
    }

    /// <summary>
    /// Retorna uma categoria pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da categoria.</param>
    /// <returns>Categoria encontrada.</returns>
    [HttpGet("{id:long}")]
    [ProducesResponseType(
        typeof(CategoryResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(long id)
    {
        logger.LogInformation(
            "Buscando categoria por ID: {CategoryId}",
            id
        );

        var category = await categoryService.GetByIdAsync(id);

        if (category is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Categoria não encontrada.",
                Detail = $"Não foi encontrada uma categoria com o ID {id}.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(category);
    }

    /// <summary>
    /// Cadastra uma nova categoria.
    /// </summary>
    /// <param name="request">Dados da categoria.</param>
    /// <returns>Categoria criada.</returns>
    [HttpPost]
    [ProducesResponseType(
        typeof(CategoryResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(
        [FromBody] CategoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        logger.LogInformation(
            "Iniciando cadastro da categoria: {CategoryName}",
            request.Name
        );

        var created = await categoryService.AddAsync(request);

        logger.LogInformation(
            "Categoria criada com sucesso: {CategoryId}",
            created.Id
        );

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created
        );
    }

    /// <summary>
    /// Atualiza uma categoria existente.
    /// </summary>
    /// <param name="id">Identificador da categoria.</param>
    /// <param name="request">Novos dados da categoria.</param>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] CategoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        logger.LogInformation(
            "Atualizando categoria: {CategoryId}",
            id
        );

        await categoryService.UpdateAsync(id, request);

        return NoContent();
    }

    /// <summary>
    /// Exclui uma categoria.
    /// </summary>
    /// <param name="id">Identificador da categoria.</param>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(long id)
    {
        logger.LogInformation(
            "Excluindo categoria: {CategoryId}",
            id
        );

        await categoryService.DeleteAsync(id);

        return NoContent();
    }
}