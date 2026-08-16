using Domain.Exceptions;
using Moq;
using OrderService.Application.Commands;
using OrderService.Domain.Contracts;
using OrderService.Domain.Enums;
using OrderService.Domain.Exceptions.DomainExceptions;
using OrderService.Domain.Orders;
using Xunit;

namespace OrderService.Application.Tests
{
    public class CancelOrderCommandHandlerTests
    {

        [Fact]
        public async Task Handle_ShouldCancelOrder_WhenOrderExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var order = new Order(userId);

            var repository = new Mock<IOrderRepository>();
            repository.Setup(x => x.GetByIdAndUserIdTrackedAsync(order.Id, userId, It.IsAny<CancellationToken>())).ReturnsAsync(order);

            var uow = new Mock<IUnitOfWork>();

            var handler = new CancelOrderCommandHandler(repository.Object, uow.Object);

            var command = new CancelOrderCommand(userId, order.Id);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order.Status);

            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }     

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenOrderDoesNotExist()
        {
            // Arrange
            var userid = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var repository = new Mock<IOrderRepository>();
            repository.Setup(x => x.GetByIdTrackedAsync(orderId, It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);
            var uow = new Mock<IUnitOfWork>();
            var handler = new CancelOrderCommandHandler(repository.Object, uow.Object);
            var command = new CancelOrderCommand(userid, orderId);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));
            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldPropagateDomainException_WhenOrderCannotBeCancelled()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var order = new Order(userId);
            var productId = Guid.NewGuid();

            order.AddItem(productId, 1);

            var productPrices = new Dictionary<Guid, (decimal UnitPrice, CurrencyCode Currency)>
            {
                [productId] = (10, CurrencyCode.USD)
            };

            order.Confirm(productPrices, DateTime.UtcNow);
            order.Complete();

            var repository = new Mock<IOrderRepository>();

            repository.Setup(x => x.GetByIdAndUserIdTrackedAsync(order.Id, userId, It.IsAny<CancellationToken>())).ReturnsAsync(order);

            var uow = new Mock<IUnitOfWork>();

            var handler = new CancelOrderCommandHandler(repository.Object, uow.Object);

            var command = new CancelOrderCommand(userId, order.Id);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOrderException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));

            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
