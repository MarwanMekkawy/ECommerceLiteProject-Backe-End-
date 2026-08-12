using AutoMapper;
using Moq;
using OrderService.Application.DTOs;
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
            repository.Setup(x => x.GetByIdUnTrackedAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync(order);

            var mapper = new Mock<IMapper>();
            var orderDto = new OrderResponseDto();
            mapper.Setup(x => x.Map<OrderResponseDto>(order)).Returns(orderDto);

            var handler = new GetOrderByIdAdminQueryHandler(repository.Object, mapper.Object);
            var query = new GetOrderByIdAdminQuery(order.Id);

            // Act
            var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Same(orderDto, result);
            repository.Verify(x => x.GetByIdUnTrackedAsync(order.Id, It.IsAny<CancellationToken>()), Times.Once);
            mapper.Verify(x => x.Map<OrderResponseDto>(order), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var repository = new Mock<IOrderRepository>();
            repository.Setup(x => x.GetByIdUnTrackedAsync(orderId, It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);

            var mapper = new Mock<IMapper>();

            var handler = new GetOrderByIdAdminQueryHandler(repository.Object, mapper.Object);
            var query = new GetOrderByIdAdminQuery(orderId);

            // Act
            var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.Null(result);
            repository.Verify(x => x.GetByIdUnTrackedAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
