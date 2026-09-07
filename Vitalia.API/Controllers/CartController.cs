using Microsoft.AspNetCore.Mvc;
using Vitalia.Application.DTOs.Cart;
using Vitalia.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace Vitalia.API.Controllers;

/// <summary>
/// Gerenciamento do carrinho de compras da Vitalia.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize]
public class CartController(
    ICartService cartService,
    ILogger<CartController> logger) : ControllerBase
{
    /// <summary>
    /// Busca o carrinho ativo de um usuário.
    /// Caso não exista, um novo carrinho é criado.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCurrentUserCart()
    {
        logger.LogInformation(
            "Buscando carrinho do usuário autenticado."
        );

        var cart = await cartService
            .GetOrCreateActiveCartAsync();

        return Ok(cart);
    }

    /// <summary>
    /// Busca um carrinho pelo ID.
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id)
    {
        logger.LogInformation(
            "Buscando carrinho: {CartId}",
            id
        );

        var cart = await cartService.GetByIdAsync(id);

        if (cart is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Carrinho não encontrado.",
                Detail = $"Não foi encontrado um carrinho com o ID {id}.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(cart);
    }

    /// <summary>
    /// Adiciona um produto ao carrinho.
    /// </summary>
    [HttpPost("{cartId:long}/items")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddItem(
        long cartId,
        [FromBody] CartItemRequest request)
    {
        logger.LogInformation(
            "Adicionando produto {ProductId} ao carrinho {CartId}.",
            request.ProductId,
            cartId
        );

        var cart = await cartService.AddItemAsync(
            cartId,
            request
        );

        return Ok(cart);
    }

    /// <summary>
    /// Atualiza a quantidade de um produto no carrinho.
    /// </summary>
    [HttpPut("{cartId:long}/items/{productId:long}")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItem(
        long cartId,
        long productId,
        [FromBody] int quantity)
    {
        logger.LogInformation(
            "Atualizando produto {ProductId} no carrinho {CartId}.",
            productId,
            cartId
        );

        var cart = await cartService.UpdateItemAsync(
            cartId,
            productId,
            quantity
        );

        return Ok(cart);
    }

    /// <summary>
    /// Remove um produto do carrinho.
    /// </summary>
    [HttpDelete("{cartId:long}/items/{productId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveItem(
        long cartId,
        long productId)
    {
        logger.LogInformation(
            "Removendo produto {ProductId} do carrinho {CartId}.",
            productId,
            cartId
        );

        await cartService.RemoveItemAsync(
            cartId,
            productId
        );

        return NoContent();
    }

    /// <summary>
    /// Finaliza o carrinho.
    /// </summary>
    [HttpPost("{cartId:long}/checkout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Checkout(long cartId)
    {
        logger.LogInformation(
            "Finalizando carrinho: {CartId}",
            cartId
        );

        await cartService.CheckoutAsync(cartId);

        return NoContent();
    }
}