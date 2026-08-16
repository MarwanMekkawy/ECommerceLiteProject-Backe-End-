using Moq;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Domain.Contracts;
using OrderService.Domain.Exceptions.DomainExceptions;
using OrderService.Domain.Orders;
using Xunit;

namespace OrderService.Application.Tests
{
    public class CreateOrderCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldCreatePendingOrder_WhenUserHasNoPendingOrder()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var items = new List<CreateOrderItemDto>
            {
                new() { ProductId = productId,  Quantity = 2 }
            };

            var repository = new Mock<IOrderRepository>();

            repository.Setup(x => x.GetPendingByUserIdTrackedAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);

            var uow = new Mock<IUnitOfWork>();

            var handler = new CreateOrderCommandHandler(repository.Object, uow.Object);

            var command = new CreateOrderCommand(userId, items);

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
        public async Task Handle_ShouldAddAllItemsToExistingPendingOrder()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId1 = Guid.NewGuid();
            var productId2 = Guid.NewGuid();

            var order = new Order(userId);

            var items = new List<CreateOrderItemDto>
            {
                new()
                {
                    ProductId = productId1,
                    Quantity = 2
                },
                new()
                {
                    ProductId = productId2,
                    Quantity = 3
                }
            };

            var repository = new Mock<IOrderRepository>();

            repository.Setup(x => x.GetPendingByUserIdTrackedAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(order);

            var uow = new Mock<IUnitOfWork>();

            var handler = new CreateOrderCommandHandler(repository.Object, uow.Object);

            var command = new CreateOrderCommand(userId, items);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(2, order.Items.Count);

            Assert.Equal(2, order.Items.First(x => x.ProductId == productId1).Quantity);

            Assert.Equal(3, order.Items.First(x => x.ProductId == productId2).Quantity);

            repository.Verify(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);

            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task Handle_ShouldIncreaseQuantity_WhenBundleContainsExistingProduct()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var order = new Order(userId);
            order.AddItem(productId, 2);

            var items = new List<CreateOrderItemDto>
            {
                new()
                {
                    ProductId = productId,
                    Quantity = 3
                }
            };

            var repository = new Mock<IOrderRepository>();

            repository.Setup(x => x.GetPendingByUserIdTrackedAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(order);

            var uow = new Mock<IUnitOfWork>();

            var handler = new CreateOrderCommandHandler(repository.Object, uow.Object);

            var command = new CreateOrderCommand(userId, items);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            var item = Assert.Single(order.Items);

            Assert.Equal(5, item.Quantity);

            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
