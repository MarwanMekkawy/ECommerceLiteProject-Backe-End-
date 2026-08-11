using Moq;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Domain.Contracts;
using OrderService.Domain.Exceptions.DomainExceptions;
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
            var command = new CreateOrderCommand(userId, [new CreateOrderItemDto { ProductId = productId, Quantity = 2 }]);

            var orderRepository = new Mock<IOrderRepository>();
            var uow = new Mock<IUnitOfWork>();

            var handler = new CreateOrderCommandHandler(
                orderRepository.Object,
                uow.Object);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            orderRepository.Verify(
                x => x.AddAsync(
                    It.Is<Order>(o =>
                        o.UserId == userId &&
                        o.Items.Count == 1 &&
                        o.Items.First().ProductId == productId &&
                        o.Items.First().Quantity == 2),
                    TestContext.Current.CancellationToken),
                Times.Once);

            uow.Verify(x => x.SaveChangesAsync(TestContext.Current.CancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenUserIdIsEmpty()
        {
            // Arrange
            var userId = Guid.Empty;
            var productId = Guid.NewGuid();

            var command = new CreateOrderCommand(userId, [new CreateOrderItemDto { ProductId = productId, Quantity = 2 }]);

            var orderRepository = new Mock<IOrderRepository>();
            var uow = new Mock<IUnitOfWork>();

            var handler = new CreateOrderCommandHandler(orderRepository.Object, uow.Object);

            // Act
            var action = () => handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            await Assert.ThrowsAsync<InvalidOrderException>(action);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenProductIdIsEmpty()
        {
            // Arrange
            var command = new CreateOrderCommand(Guid.NewGuid(), [new CreateOrderItemDto { ProductId = Guid.Empty, Quantity = 2 }]);

            var orderRepository = new Mock<IOrderRepository>();
            var uow = new Mock<IUnitOfWork>();

            var handler = new CreateOrderCommandHandler(orderRepository.Object, uow.Object);

            // Act
            var action = () => handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            await Assert.ThrowsAsync<InvalidOrderItemException>(action);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenQuantityIsZero()
        {
            // Arrange
            var command = new CreateOrderCommand(Guid.NewGuid(), [new CreateOrderItemDto { ProductId = Guid.NewGuid(), Quantity = 0 }]);

            var orderRepository = new Mock<IOrderRepository>();
            var uow = new Mock<IUnitOfWork>();

            var handler = new CreateOrderCommandHandler(orderRepository.Object, uow.Object);

            // Act
            var action = () => handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            await Assert.ThrowsAsync<InvalidOrderItemException>(action);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenQuantityIsNegative()
        {
            // Arrange
            var command = new CreateOrderCommand(Guid.NewGuid(), [new CreateOrderItemDto { ProductId = Guid.NewGuid(), Quantity = -1 }]);

            var orderRepository = new Mock<IOrderRepository>();
            var uow = new Mock<IUnitOfWork>();

            var handler = new CreateOrderCommandHandler(orderRepository.Object, uow.Object);

            // Act
            var action = () => handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            await Assert.ThrowsAsync<InvalidOrderItemException>(action);
        }
    }
}
