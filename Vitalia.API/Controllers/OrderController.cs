using Microsoft.AspNetCore.Mvc;
using Vitalia.Application.DTOs.Order;
using Vitalia.Application.Interfaces.Services;

namespace Vitalia.API.Controllers;

/// <summary>
/// Controller responsável pelo gerenciamento de pedidos.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class OrderController(
    IOrderService orderService,
    ILogger<OrderController> logger) : ControllerBase
{
    /// <summary>
    /// Busca um pedido pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador do pedido.</param>
    /// <returns>Os dados do pedido encontrado.</returns>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id)
    {
        logger.LogInformation("Buscando pedido: {OrderId}", id);

        var order = await orderService.GetByIdAsync(id);

        if (order is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Pedido não encontrado.",
                Detail = $"Não foi encontrado um pedido com o ID {id}.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(order);
    }

    /// <summary>
    /// Busca todos os pedidos de um usuário.
    /// </summary>
    /// <param name="userId">Identificador do usuário.</param>
    /// <returns>Lista de pedidos pertencentes ao usuário.</returns>
    [HttpGet("user/{userId:long}")]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUser(long userId)
    {
        logger.LogInformation(
            "Buscando pedidos do usuário: {UserId}",
            userId);

        var orders = await orderService.GetByUserIdAsync(userId);

        return Ok(orders);
    }

    /// <summary>
    /// Finaliza o carrinho e cria um novo pedido.
    /// </summary>
    /// <param name="cartId">Identificador do carrinho que será finalizado.</param>
    /// <returns>O pedido criado a partir do carrinho.</returns>
    [HttpPost("checkout/{cartId:long}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Checkout(long cartId)
    {
        logger.LogInformation(
            "Realizando checkout do carrinho: {CartId}",
            cartId);

        var order = await orderService.CheckoutAsync(cartId);

        return Ok(order);
    }
    
    /// <summary>
    /// Confirma um pedido que está aguardando confirmação.
    /// </summary>
    /// <param name="id">Identificador do pedido.</param>
    /// <returns>Retorna 204 quando o pedido é confirmado.</returns>
    [HttpPut("{id:long}/confirm")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Confirm(long id)
    {
        await orderService.ConfirmAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Inicia o processamento de um pedido confirmado.
    /// </summary>
    /// <param name="id">Identificador do pedido.</param>
    /// <returns>Retorna 204 quando o pedido é colocado em processamento.</returns>
    [HttpPut("{id:long}/process")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Process(long id)
    {
        await orderService.ProcessAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Marca um pedido em processamento como enviado.
    /// </summary>
    /// <param name="id">Identificador do pedido.</param>
    /// <returns>Retorna 204 quando o pedido é enviado.</returns>
    [HttpPut("{id:long}/ship")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Ship(long id)
    {
        await orderService.ShipAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Marca um pedido enviado como entregue.
    /// </summary>
    /// <param name="id">Identificador do pedido.</param>
    /// <returns>Retorna 204 quando o pedido é marcado como entregue.</returns>
    [HttpPut("{id:long}/deliver")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deliver(long id)
    {
        await orderService.DeliverAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Cancela um pedido.
    /// </summary>
    /// <param name="id">Identificador do pedido.</param>
    /// <returns>Retorna 204 quando o pedido é cancelado.</returns>
    [HttpPut("{id:long}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(long id)
    {
        await orderService.CancelAsync(id);
        return NoContent();
    }
}