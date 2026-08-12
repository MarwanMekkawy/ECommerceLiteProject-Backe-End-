using Moq;
using OrderService.Application.Queries;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OrderService.Application.Tests
{
    public class GetLatestOrderQueryHandlerTests
        {
        [Fact]
        public async Task Handle_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var order = new Order(userId);

            var repository = new Mock<IOrderRepository>();

            repository.Setup(x => x.GetLatestByUserIdUnTrackedAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(order);

            var handler = new GetLatestOrderQueryHandler(repository.Object);

            var query = new GetLatestOrderQuery(userId);

            // Act
            var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Same(order, result);
            repository.Verify(x => x.GetLatestByUserIdUnTrackedAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var repository = new Mock<IOrderRepository>();

            repository.Setup(x => x.GetLatestByUserIdUnTrackedAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);

            var handler = new GetLatestOrderQueryHandler(repository.Object);

            var query = new GetLatestOrderQuery(userId);

            // Act
            var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.Null(result);
            repository.Verify(x => x.GetLatestByUserIdUnTrackedAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
