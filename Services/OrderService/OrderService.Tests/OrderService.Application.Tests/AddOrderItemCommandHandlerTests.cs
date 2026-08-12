using Domain.Exceptions;
using Moq;
using OrderService.Application.Commands;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;
using Xunit;

namespace OrderService.Application.Tests
{
    public class AddOrderItemCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldAddItem_WhenOrderExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var order = new Order(userId);
            var productId = Guid.NewGuid();

            var repository = new Mock<IOrderRepository>();

            repository.Setup(x => x.GetByIdAndUserIdTrackedAsync(order.Id, userId, It.IsAny<CancellationToken>())).ReturnsAsync(order);

            var uow = new Mock<IUnitOfWork>();

            var handler = new AddOrderItemCommandHandler(repository.Object, uow.Object);

            var command = new AddOrderItemCommand(userId, order.Id, productId, 2);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.Single(order.Items);
            Assert.Equal(productId, order.Items.First().ProductId);
            Assert.Equal(2, order.Items.First().Quantity);

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

            var handler = new AddOrderItemCommandHandler(repository.Object, uow.Object);

            var command = new AddOrderItemCommand(userId, orderId, Guid.NewGuid(), 2);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));

            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
