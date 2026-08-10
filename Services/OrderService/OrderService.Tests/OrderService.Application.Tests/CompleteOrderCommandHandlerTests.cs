using Domain.Exceptions;
using Moq;
using OrderService.Application.Commands;
using OrderService.Domain.Contracts;
using OrderService.Domain.Exceptions.DomainExceptions;
using OrderService.Domain.Orders;
using Xunit;

namespace OrderService.Application.Tests
{
    public class CompleteOrderCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldCompleteOrder_WhenOrderIsConfirmed()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.Confirm();
            var repository = new Mock<IOrderRepository>();
            repository.Setup(x => x.GetByIdTrackedAsync(order.Id,It.IsAny<CancellationToken>())).ReturnsAsync(order);
            var uow = new Mock<IUnitOfWork>();
            var handler = new CompleteOrderCommandHandler(repository.Object,uow.Object);
            var command = new CompleteOrderCommand(order.Id);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(OrderStatus.Completed, order.Status);
            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldThrow_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var repository = new Mock<IOrderRepository>();
            repository.Setup(x => x.GetByIdTrackedAsync(orderId, It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);
            var uow = new Mock<IUnitOfWork>();
            var handler = new CompleteOrderCommandHandler(repository.Object, uow.Object);
            var command = new CompleteOrderCommand(orderId);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));
            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
        [Fact]
        public async Task Handle_ShouldPropagateDomainException_WhenOrderCannotBeCompleted()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var repository = new Mock<IOrderRepository>();
            repository.Setup(x => x.GetByIdTrackedAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync(order);
            var uow = new Mock<IUnitOfWork>();
            var handler = new CompleteOrderCommandHandler(repository.Object, uow.Object);
            var command = new CompleteOrderCommand(order.Id);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOrderException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));
            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
