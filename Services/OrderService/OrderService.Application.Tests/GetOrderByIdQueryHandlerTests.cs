using Moq;
using OrderService.Application.Queries;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;
using Xunit;

namespace OrderService.Application.Tests
{
    public class GetOrderByIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var order = new Order(userId);

            var repository = new Mock<IOrderRepository>();

            repository.Setup(x => x.GetByIdUntrackedAsync(orderId, It.IsAny<CancellationToken>())).ReturnsAsync(order);

            var handler = new GetOrderByIdQueryHandler(repository.Object);

            var query = new GetOrderByIdQuery(orderId);

            // Act
            var result = await handler.HandleAsync(query,TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Same(order, result);
            repository.Verify(x => x.GetByIdUntrackedAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            var repository = new Mock<IOrderRepository>();

            repository.Setup(x => x.GetByIdUntrackedAsync(orderId,It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);

            var handler = new GetOrderByIdQueryHandler(repository.Object);

            var query = new GetOrderByIdQuery(orderId);

            // Act
            var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.Null(result);
            repository.Verify(x => x.GetByIdUntrackedAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
