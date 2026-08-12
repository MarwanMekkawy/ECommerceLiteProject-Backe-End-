using AutoMapper;
using Moq;
using OrderService.Application.DTOs;
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

            repository.Setup(x => x.GetByIdAndUserIdUnTrackedAsync(orderId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(order);

            var mapper = new Mock<IMapper>();
            var orderDto = new OrderResponseDto();

            mapper.Setup(x => x.Map<OrderResponseDto>(order)).Returns(orderDto);

            var handler = new GetOrderByIdQueryHandler(repository.Object, mapper.Object);

            var query = new GetOrderByIdQuery(orderId, userId);

            // Act
            var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Same(orderDto, result);
            repository.Verify(x => x.GetByIdAndUserIdUnTrackedAsync(orderId, userId, It.IsAny<CancellationToken>()), Times.Once);
            mapper.Verify(x => x.Map<OrderResponseDto>(order), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmpty_WhenUserHasNoOrders()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var repository = new Mock<IOrderRepository>();
            repository.Setup(x => x.GetPagedByUserIdAsync(userId, 1, 10, It.IsAny<CancellationToken>())).ReturnsAsync([]);

            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<IReadOnlyList<OrderResponseDto>>(It.IsAny<IReadOnlyList<Order>>())).Returns([]);

            var handler = new GetOrdersByUserQueryHandler(repository.Object, mapper.Object);
            var query = new GetOrdersByUserQuery(userId, 1, 10);

            // Act
            var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.Empty(result);
            repository.Verify(x => x.GetPagedByUserIdAsync(userId, 1, 10, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
