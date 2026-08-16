using AutoMapper;
using Moq;
using OrderService.Application.DTOs;
using OrderService.Application.Queries;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;
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

            var mapper = new Mock<IMapper>();
            var orderDto = new OrderResponseDto();

            mapper.Setup(x => x.Map<OrderResponseDto>(order)).Returns(orderDto);

            var handler = new GetLatestOrderQueryHandler(repository.Object, mapper.Object);

            var query = new GetLatestOrderQuery(userId);

            // Act
            var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Same(orderDto, result);
            repository.Verify(x => x.GetLatestByUserIdUnTrackedAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            mapper.Verify(x => x.Map<OrderResponseDto>(order), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var repository = new Mock<IOrderRepository>();

            repository.Setup(x => x.GetLatestByUserIdUnTrackedAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);

            var mapper = new Mock<IMapper>();

            var handler = new GetLatestOrderQueryHandler(repository.Object, mapper.Object);

            var query = new GetLatestOrderQuery(userId);

            // Act
            var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.Null(result);
            repository.Verify(x => x.GetLatestByUserIdUnTrackedAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
