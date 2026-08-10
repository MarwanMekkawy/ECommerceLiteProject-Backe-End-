using Moq;
using OrderService.Application.Queries;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;
using Xunit;

namespace OrderService.Application.Tests
{
    public class GetOrderByIdAdminQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var repository = new Mock<IOrderRepository>();
            repository.Setup(x => x.GetByIdUntrackedAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync(order);
            var handler = new GetOrderByIdAdminQueryHandler(repository.Object);
            var query = new GetOrderByIdAdminQuery(order.Id);

            // Act
            var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Same(order, result);
            repository.Verify(x => x.GetByIdUntrackedAsync(order.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var repository = new Mock<IOrderRepository>();
            repository.Setup(x => x.GetByIdUntrackedAsync(orderId, It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);
            var handler = new GetOrderByIdAdminQueryHandler(repository.Object);
            var query = new GetOrderByIdAdminQuery(orderId);

            // Act
            var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.Null(result);
            repository.Verify(x => x.GetByIdUntrackedAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
