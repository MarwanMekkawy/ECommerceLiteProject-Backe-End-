using Moq;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Domain.Orders;
using Xunit;

namespace OrderService.Application.Tests
{
    public class CreateOrderCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldCreateOrder_WhenCommandIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var command = new CreateOrderCommand
            {
                UserId = userId,
                Items = [new CreateOrderItemDto { ProductId = productId, Quantity = 2 }]
            };
            var orderRepository = new Mock<IOrderRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var handler = new CreateOrderCommandHandler(orderRepository.Object, unitOfWork.Object);

            // Act
            await handler.Handle(command);

            // Assert
            orderRepository.Verify(
                x => x.AddAsync(
                    It.Is<Order>(o =>
                    o.UserId == userId &&
                    o.Items.Count == 1 &&
                    o.Items.Single().ProductId == productId &&
                    o.Items.Single().Quantity == 2)),
                Times.Once);

            unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
}
