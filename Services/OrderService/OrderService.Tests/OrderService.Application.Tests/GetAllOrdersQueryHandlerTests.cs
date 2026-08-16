using AutoMapper;
using Moq;
using OrderService.Application.DTOs;
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

            var mapper = new Mock<IMapper>();
            var orderDtos = new List<OrderResponseDto> { new OrderResponseDto(), new OrderResponseDto() };
            mapper.Setup(x => x.Map<IReadOnlyList<OrderResponseDto>>(orders)).Returns(orderDtos);

            var handler = new GetAllOrdersQueryHandler(repository.Object, mapper.Object);
            var query = new GetAllOrdersQuery(1, 10);

            // Act
            var result = await handler.HandleAsync(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Same(orderDtos[0], result[0]);
            Assert.Same(orderDtos[1], result[1]);

            repository.Verify(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()), Times.Once);
            mapper.Verify(x => x.Map<IReadOnlyList<OrderResponseDto>>(orders), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmpty_WhenNoOrdersExist()
        {
            // Arrange
            var repository = new Mock<IOrderRepository>();
            repository.Setup(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync([]);

            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<IReadOnlyList<OrderResponseDto>>(It.IsAny<IReadOnlyList<Order>>())).Returns([]);

            var handler = new GetAllOrdersQueryHandler(repository.Object, mapper.Object);
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

            var mapper = new Mock<IMapper>();
            mapper.Setup(x => x.Map<IReadOnlyList<OrderResponseDto>>(It.IsAny<List<Order>>())).Returns([]);

            var handler = new GetAllOrdersQueryHandler(repository.Object, mapper.Object);
            var query = new GetAllOrdersQuery(3, 20);

            // Act
            await handler.HandleAsync(query, TestContext.Current.CancellationToken);

            // Assert
            repository.Verify(x => x.GetPagedAsync(3, 20, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
