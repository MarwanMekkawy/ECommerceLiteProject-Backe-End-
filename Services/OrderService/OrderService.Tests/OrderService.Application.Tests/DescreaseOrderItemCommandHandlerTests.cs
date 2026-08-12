using Domain.Exceptions;
using Moq;
using OrderService.Application.Commands;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;
using Xunit;

namespace OrderService.Application.Tests
{
    public class DescreaseOrderItemCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldDecreaseItemQuantity_WhenOrderExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var order = new Order(userId);
            var productId = Guid.NewGuid();

            order.AddItem(productId, 5);

            var repository = new Mock<IOrderRepository>();

            repository.Setup(x => x.GetByIdAndUserIdTrackedAsync(order.Id, userId, It.IsAny<CancellationToken>())).ReturnsAsync(order);

            var uow = new Mock<IUnitOfWork>();

            var handler = new DecreaseOrderItemCommandHandler(repository.Object, uow.Object);

            var command = new DecreaseOrderItemCommand(userId, order.Id, productId, 2);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.Single(order.Items);
            Assert.Equal(3, order.Items.First().Quantity);

            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldRemoveItem_WhenQuantityReachesZero()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var order = new Order(userId);
            var productId = Guid.NewGuid();

            order.AddItem(productId, 2);

            var repository = new Mock<IOrderRepository>();

            repository.Setup(x => x.GetByIdAndUserIdTrackedAsync(order.Id, userId, It.IsAny<CancellationToken>())).ReturnsAsync(order);

            var uow = new Mock<IUnitOfWork>();

            var handler = new DecreaseOrderItemCommandHandler(repository.Object, uow.Object);

            var command = new DecreaseOrderItemCommand(userId, order.Id, productId, 2);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.Empty(order.Items);

            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenOrderDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            var repository = new Mock<IOrderRepository>();

            repository.Setup(x => x.GetByIdAndUserIdTrackedAsync(orderId, userId, It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);

            var uow = new Mock<IUnitOfWork>();

            var handler = new DecreaseOrderItemCommandHandler(repository.Object, uow.Object);

            var command = new DecreaseOrderItemCommand(userId, orderId, Guid.NewGuid(), 1);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));

            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
