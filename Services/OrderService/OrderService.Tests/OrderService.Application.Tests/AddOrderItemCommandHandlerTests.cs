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
        public async Task Handle_ShouldAddItemToExistingPendingOrder()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var order = new Order(userId);

            var repository = new Mock<IOrderRepository>();

            repository.Setup(x => x.GetPendingByUserIdTrackedAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(order);

            var uow = new Mock<IUnitOfWork>();

            var handler = new AddOrderItemCommandHandler(repository.Object, uow.Object);

            var command = new AddOrderItemCommand(userId, productId, 2);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            var item = Assert.Single(order.Items);

            Assert.Equal(productId, item.ProductId);
            Assert.Equal(2, item.Quantity);

            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task Handle_ShouldCreatePendingOrder_WhenUserHasNoPendingOrder()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var repository = new Mock<IOrderRepository>();

            repository.Setup(x => x.GetPendingByUserIdTrackedAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);

            var uow = new Mock<IUnitOfWork>();

            var handler = new AddOrderItemCommandHandler(repository.Object, uow.Object);

            var command = new AddOrderItemCommand(userId, productId, 2);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            repository.Verify(
                x => x.AddAsync(
                    It.Is<Order>(o =>
                        o.UserId == userId &&
                        o.Status == OrderStatus.Pending &&
                        o.Items.Count == 1 &&
                        o.Items.First().ProductId == productId &&
                        o.Items.First().Quantity == 2),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task Handle_ShouldIncreaseQuantity_WhenProductAlreadyExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var order = new Order(userId);
            order.AddItem(productId, 2);

            var repository = new Mock<IOrderRepository>();

            repository.Setup(x => x.GetPendingByUserIdTrackedAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(order);

            var uow = new Mock<IUnitOfWork>();

            var handler = new AddOrderItemCommandHandler(repository.Object, uow.Object);

            var command = new AddOrderItemCommand(userId, productId, 3);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            var item = Assert.Single(order.Items);

            Assert.Equal(5, item.Quantity);

            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
