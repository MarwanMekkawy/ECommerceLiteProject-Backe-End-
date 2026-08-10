using Moq;
using OrderService.Application.Queries;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;
using Xunit;

namespace OrderService.Application.Tests
{
    public class GetAllOrdersQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnOrders()
        {
            // Arrange
            var orders = new List<Order> { new Order(Guid.NewGuid()), new Order(Guid.NewGuid()) };
            var repository = new Mock<IOrderRepository>();
            repository.Setup(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(orders);
            var handler = new GetAllOrdersQueryHandler(repository.Object);
            var query = new GetAllOrdersQuery(1, 10);

            // Act
            var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Same(orders[0], result[0]);
            Assert.Same(orders[1], result[1]);

            repository.Verify(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmpty_WhenNoOrdersExist()
        {
            // Arrange
            var repository = new Mock<IOrderRepository>();
            repository.Setup(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync([]);
            var handler = new GetAllOrdersQueryHandler(repository.Object);
            var query = new GetAllOrdersQuery(1, 10);

            // Act
            var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.Empty(result);
            repository.Verify(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassPaginationParametersToRepository()
        {
            // Arrange
            var repository = new Mock<IOrderRepository>();
            repository.Setup(x => x.GetPagedAsync(3, 20, It.IsAny<CancellationToken>())).ReturnsAsync([]);
            var handler = new GetAllOrdersQueryHandler(repository.Object);
            var query = new GetAllOrdersQuery(3, 20);

            // Act
            await handler.HandleAsync(query, TestContext.Current.CancellationToken);

            // Assert
            repository.Verify(x => x.GetPagedAsync(3, 20, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
