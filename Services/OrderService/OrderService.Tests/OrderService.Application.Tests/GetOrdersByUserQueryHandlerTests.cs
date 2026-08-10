using Moq;
using OrderService.Application.Queries;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;
using Xunit;

namespace OrderService.Application.Tests
{
    public class GetOrdersByUserQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnUserOrders()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orders = new List<Order> {new Order(userId),new Order(userId)};
            var repository = new Mock<IOrderRepository>();

            repository.Setup(x => x.GetPagedByUserIdAsync(userId, 1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(orders);
            var handler = new GetOrdersByUserQueryHandler(repository.Object);
            var query = new GetOrdersByUserQuery(userId, 1, 10);

            // Act
            var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Same(orders[0], result[0]);
            Assert.Same(orders[1], result[1]);
            repository.Verify(x => x.GetPagedByUserIdAsync(userId, 1, 10, It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldReturnEmpty_WhenUserHasNoOrders()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var repository = new Mock<IOrderRepository>();
            repository.Setup(x => x.GetPagedByUserIdAsync(userId, 1, 10, It.IsAny<CancellationToken>())).ReturnsAsync([]);
            var handler = new GetOrdersByUserQueryHandler(repository.Object);
            var query = new GetOrdersByUserQuery(userId, 1, 10);

            // Act
            var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.Empty(result);
            repository.Verify(x => x.GetPagedByUserIdAsync(userId, 1, 10, It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldPassPaginationParametersToRepository()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var repository = new Mock<IOrderRepository>();
            repository.Setup(x => x.GetPagedByUserIdAsync(userId, 2, 5, It.IsAny<CancellationToken>())).ReturnsAsync([]);
            var handler = new GetOrdersByUserQueryHandler(repository.Object);
            var query = new GetOrdersByUserQuery(userId, 2, 5);

            // Act
            await handler.HandleAsync(query, TestContext.Current.CancellationToken);

            // Assert
            repository.Verify(x => x.GetPagedByUserIdAsync(userId, 2, 5, It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
