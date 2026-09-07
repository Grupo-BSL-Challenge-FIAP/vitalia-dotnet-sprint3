using Vitalia.Domain.Entities;
using Vitalia.Domain.Enums;

namespace Vitalia.Domain.Tests;

public class OrderTests
{
    [Fact]
    public void CriarOrder_ComUserIdValido_DeveCriarPedidoPendente()
    {
        // Arrange
        const long userId = 4;

        // Act
        var order = new Order(userId);

        // Assert
        Assert.Equal(userId, order.UserId);
        Assert.Equal(OrderStatus.PENDING, order.Status);
        Assert.Equal(0, order.TotalAmount);
        Assert.Empty(order.Items);
    }
    
    [Fact]
    public void ConfirmarOrder_Pendente_DeveAlterarStatusParaConfirmado()
    {
        // Arrange
        var order = new Order(4);

        // Act
        order.Confirm();

        // Assert
        Assert.Equal(OrderStatus.CONFIRMED, order.Status);
    }
    
    [Fact]
    public void ProcessarOrder_Confirmado_DeveAlterarStatusParaProcessando()
    {
        // Arrange
        var order = new Order(4);
        order.Confirm();

        // Act
        order.Process();

        // Assert
        Assert.Equal(OrderStatus.PROCESSING, order.Status);
    }
    
    [Fact]
    public void EnviarOrder_Processando_DeveAlterarStatusParaEnviado()
    {
        // Arrange
        var order = new Order(4);
        order.Confirm();
        order.Process();

        // Act
        order.Ship();

        // Assert
        Assert.Equal(OrderStatus.SHIPPED, order.Status);
    }
    
    [Fact]
    public void EntregarOrder_Enviado_DeveAlterarStatusParaEntregue()
    {
        // Arrange
        var order = new Order(4);
        order.Confirm();
        order.Process();
        order.Ship();

        // Act
        order.Deliver();

        // Assert
        Assert.Equal(OrderStatus.DELIVERED, order.Status);
    }
    
    [Fact]
    public void ProcessarOrder_Pendente_DeveLancarExcecao()
    {
        // Arrange
        var order = new Order(4);

        // Act + Assert
        Assert.Throws<InvalidOperationException>(() =>
            order.Process()
        );
    }
    
    [Fact]
    public void EnviarOrder_Confirmado_DeveLancarExcecao()
    {
        // Arrange
        var order = new Order(4);
        order.Confirm();

        // Act + Assert
        Assert.Throws<InvalidOperationException>(() =>
            order.Ship()
        );
    }
    
    [Fact]
    public void EntregarOrder_Processando_DeveLancarExcecao()
    {
        // Arrange
        var order = new Order(4);
        order.Confirm();
        order.Process();

        // Act + Assert
        Assert.Throws<InvalidOperationException>(() =>
            order.Deliver()
        );
    }
    
    [Fact]
    public void CancelarOrder_Entregue_DeveLancarExcecao()
    {
        // Arrange
        var order = new Order(4);

        order.Confirm();
        order.Process();
        order.Ship();
        order.Deliver();

        // Act + Assert
        Assert.Throws<InvalidOperationException>(() =>
            order.Cancel()
        );
    }
    
    [Fact]
    public void CancelarOrder_Pendente_DeveAlterarStatusParaCancelado()
    {
        // Arrange
        var order = new Order(4);

        // Act
        order.Cancel();

        // Assert
        Assert.Equal(OrderStatus.CANCELLED, order.Status);
    }
}