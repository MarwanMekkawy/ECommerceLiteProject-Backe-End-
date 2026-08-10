using Moq;
using OrderService.Application.Commands;
using OrderService.Domain.Contracts;
using OrderService.Domain.Exceptions.DomainExceptions;
using OrderService.Domain.Orders;
using Xunit;

namespace OrderService.Application.Tests
{
    public class ConfirmOrderCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldConfirmOrder_WhenOrderExists()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var repository = new Mock<IOrderRepository>();
            repository.Setup(x => x.GetByIdTrackedAsync(order.Id,It.IsAny<CancellationToken>())).ReturnsAsync(order);
            var uow = new Mock<IUnitOfWork>();
            var handler = new ConfirmOrderCommandHandler(repository.Object, uow.Object);
            var command = new ConfirmOrderCommand(order.Id);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(OrderStatus.Confirmed, order.Status);

            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var repository = new Mock<IOrderRepository>();
            repository.Setup(x => x.GetByIdTrackedAsync(orderId,It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);
            var uow = new Mock<IUnitOfWork>();
            var handler = new ConfirmOrderCommandHandler(repository.Object, uow.Object);
            var command = new ConfirmOrderCommand(orderId);

            // Act & Assert
            await Assert.ThrowsAsync<OrderNotFoundException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));
            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldPropagateDomainException_WhenOrderCannotBeConfirmed()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.Cancel();
            var repository = new Mock<IOrderRepository>();
            repository.Setup(x => x.GetByIdTrackedAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync(order);
            var uow = new Mock<IUnitOfWork>();
            var handler = new ConfirmOrderCommandHandler(repository.Object, uow.Object);
            var command = new ConfirmOrderCommand(order.Id);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOrderException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));
            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
