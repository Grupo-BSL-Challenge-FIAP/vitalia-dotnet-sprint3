using Microsoft.AspNetCore.Authorization;
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
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id)
    {
        logger.LogInformation(
            "Buscando pedido: {OrderId}",
            id);

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
    /// Busca todos os pedidos do usuário autenticado.
    /// </summary>
    /// <returns>Lista de pedidos pertencentes ao usuário autenticado.</returns>
    [HttpGet("my-orders")]
    [ProducesResponseType(
        typeof(IEnumerable<OrderResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUserOrders()
    {
        logger.LogInformation(
            "Buscando pedidos do usuário autenticado.");

        var orders = await orderService.GetCurrentUserOrdersAsync();

        return Ok(orders);
    }

    /// <summary>
    /// Finaliza o carrinho e cria um novo pedido.
    /// </summary>
    /// <param name="cartId">
    /// Identificador do carrinho que será finalizado.
    /// </param>
    /// <returns>O pedido criado a partir do carrinho.</returns>
    [HttpPost("checkout/{cartId:long}")]
    [ProducesResponseType(
        typeof(OrderResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
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
    /// Apenas administradores podem confirmar pedidos.
    /// </summary>
    /// <param name="id">Identificador do pedido.</param>
    /// <returns>Retorna 204 quando o pedido é confirmado.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:long}/confirm")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Confirm(long id)
    {
        logger.LogInformation(
            "Confirmando pedido: {OrderId}",
            id);

        await orderService.ConfirmAsync(id);

        return NoContent();
    }

    /// <summary>
    /// Inicia o processamento de um pedido confirmado.
    /// Apenas administradores podem processar pedidos.
    /// </summary>
    /// <param name="id">Identificador do pedido.</param>
    /// <returns>Retorna 204 quando o pedido é colocado em processamento.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:long}/process")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Process(long id)
    {
        logger.LogInformation(
            "Processando pedido: {OrderId}",
            id);

        await orderService.ProcessAsync(id);

        return NoContent();
    }

    /// <summary>
    /// Marca um pedido em processamento como enviado.
    /// Apenas administradores podem enviar pedidos.
    /// </summary>
    /// <param name="id">Identificador do pedido.</param>
    /// <returns>Retorna 204 quando o pedido é enviado.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:long}/ship")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Ship(long id)
    {
        logger.LogInformation(
            "Enviando pedido: {OrderId}",
            id);

        await orderService.ShipAsync(id);

        return NoContent();
    }

    /// <summary>
    /// Marca um pedido enviado como entregue.
    /// Apenas administradores podem entregar pedidos.
    /// </summary>
    /// <param name="id">Identificador do pedido.</param>
    /// <returns>Retorna 204 quando o pedido é marcado como entregue.</returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:long}/deliver")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Deliver(long id)
    {
        logger.LogInformation(
            "Entregando pedido: {OrderId}",
            id);

        await orderService.DeliverAsync(id);

        return NoContent();
    }

    /// <summary>
    /// Cancela um pedido pertencente ao usuário autenticado.
    /// </summary>
    /// <param name="id">Identificador do pedido.</param>
    /// <returns>Retorna 204 quando o pedido é cancelado.</returns>
    [HttpPut("{id:long}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cancel(long id)
    {
        logger.LogInformation(
            "Cancelando pedido: {OrderId}",
            id);

        await orderService.CancelAsync(id);

        return NoContent();
    }
}