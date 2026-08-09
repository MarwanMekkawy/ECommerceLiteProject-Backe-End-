using Domain.Exceptions;
using Moq;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
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
        [Fact]
        public async Task Handle_ShouldThrow_WhenUserIdIsEmpty()
        {
            // Arrange
            var userId = Guid.Empty;
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
            var action = () => handler.Handle(command);

            // Assert
            await Assert.ThrowsAsync<InvalidOrderException>(action);
        }
        [Fact]
        public async Task Handle_ShouldThrow_WhenProductIdIsEmpty()
        {
            // Arrange
            var command = new CreateOrderCommand
                            {
                                UserId = Guid.NewGuid(),
                                Items = [new CreateOrderItemDto { ProductId = Guid.Empty, Quantity = 2 }]
                            };
            var orderRepository = new Mock<IOrderRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var handler = new CreateOrderCommandHandler(orderRepository.Object, unitOfWork.Object);
            // Act
            var action = () => handler.Handle(command);

            // Assert
            await Assert.ThrowsAsync<InvalidOrderItemException>(action);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenQuantityIsZero()
        {
            // Arrange
            var command = new CreateOrderCommand
                            {
                                UserId = Guid.NewGuid(),
                                Items = [new CreateOrderItemDto { ProductId = Guid.NewGuid(), Quantity = 0 }]
                            };
            var orderRepository = new Mock<IOrderRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var handler = new CreateOrderCommandHandler(orderRepository.Object, unitOfWork.Object);

            // Act
            var action = () => handler.Handle(command);

            // Assert
            await Assert.ThrowsAsync<InvalidOrderItemException>(action);
        }
        [Fact]
        public async Task Handle_ShouldThrow_WhenQuantityIsNegative()
        {
            // Arrange
            var command = new CreateOrderCommand
                            {
                                UserId = Guid.NewGuid(),
                                Items = [new CreateOrderItemDto { ProductId = Guid.NewGuid(), Quantity = -1 }]
                            };
            var orderRepository = new Mock<IOrderRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var handler = new CreateOrderCommandHandler(orderRepository.Object, unitOfWork.Object);

            // Act
            var action = () => handler.Handle(command);

            // Assert
            await Assert.ThrowsAsync<InvalidOrderItemException>(action);
        }
    }
}
