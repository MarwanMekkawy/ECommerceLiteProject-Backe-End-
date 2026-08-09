using Moq;
using OrderService.Application.Abstractions;
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
            var productServiceClient = new Mock<IProductServiceClient>();
            var uow = new Mock<IUnitOfWork>();

            var handler = new CreateOrderCommandHandler(
                orderRepository.Object,
                productServiceClient.Object,
                uow.Object);

            // Act
            await handler.Handle(command, TestContext.Current.CancellationToken);

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

            uow.Verify(
                x => x.SaveChangesAsync(TestContext.Current.CancellationToken),Times.Once);
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
            var productService = new Mock<IProductServiceClient>();
            var handler = new CreateOrderCommandHandler(orderRepository.Object, productService.Object, unitOfWork.Object);

            // Act
            var action = () => handler.Handle(command, TestContext.Current.CancellationToken);

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
            var productService = new Mock<IProductServiceClient>();
            var handler = new CreateOrderCommandHandler(orderRepository.Object, productService.Object, unitOfWork.Object);

            // Act
            var action = () => handler.Handle(command, TestContext.Current.CancellationToken);

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
            var productService = new Mock<IProductServiceClient>();
            var handler = new CreateOrderCommandHandler(orderRepository.Object, productService.Object, unitOfWork.Object);

            // Act
            var action = () => handler.Handle(command, TestContext.Current.CancellationToken);

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
            var productService = new Mock<IProductServiceClient>();
            var handler = new CreateOrderCommandHandler(orderRepository.Object, productService.Object, unitOfWork.Object);

            // Act
            var action = () => handler.Handle(command, TestContext.Current.CancellationToken);

            // Assert
            await Assert.ThrowsAsync<InvalidOrderItemException>(action);
        }
        [Fact]
        public async Task Handle_ShouldDecreaseStock_WhenOrderIsValid()
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
            var productService = new Mock<IProductServiceClient>();
            var handler = new CreateOrderCommandHandler(orderRepository.Object, productService.Object, unitOfWork.Object);

            // Act
            await handler.Handle(command, TestContext.Current.CancellationToken);

            // Assert
            productService.Verify(x => x.DecreaseStockAsync(productId, 2),Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldNotCreateOrder_WhenStockDecreaseFails()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new CreateOrderCommand
            {
                UserId = Guid.NewGuid(),
                Items =[new CreateOrderItemDto{ProductId = productId,Quantity = 2}]
            };
            var orderRepository = new Mock<IOrderRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var productService = new Mock<IProductServiceClient>();
            productService.Setup(x => x.DecreaseStockAsync(productId, 2)).ThrowsAsync(new HttpRequestException());
            var handler = new CreateOrderCommandHandler(orderRepository.Object, productService.Object, unitOfWork.Object);

            // Act
            var action = () => handler.Handle(command, TestContext.Current.CancellationToken);

            // Assert
            await Assert.ThrowsAsync<HttpRequestException>(action);
            orderRepository.Verify(x => x.AddAsync(It.IsAny<Order>(), TestContext.Current.CancellationToken), Times.Never);
            unitOfWork.Verify(x => x.SaveChangesAsync(TestContext.Current.CancellationToken), Times.Never);
        }
        [Fact]
        public async Task Handle_ShouldRestorePreviousStock_WhenLaterStockDecreaseFails()
        {
            // Arrange
            var productA = Guid.NewGuid();
            var productB = Guid.NewGuid();

            var command = new CreateOrderCommand
            {
                UserId = Guid.NewGuid(),
                Items = [new CreateOrderItemDto { ProductId = productA, Quantity = 2 }, new CreateOrderItemDto { ProductId = productB, Quantity = 3 }]
            };
            var orderRepository = new Mock<IOrderRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var productService = new Mock<IProductServiceClient>();
            productService.Setup(x => x.DecreaseStockAsync(productA, 2)).Returns(Task.CompletedTask);
            productService.Setup(x => x.DecreaseStockAsync(productB, 3)).ThrowsAsync(new HttpRequestException());
            var handler = new CreateOrderCommandHandler(orderRepository.Object, productService.Object, unitOfWork.Object);

            // Act
            var action = () => handler.Handle(command, TestContext.Current.CancellationToken);

            // Assert
            await Assert.ThrowsAsync<HttpRequestException>(action);
            productService.Verify(x => x.IncreaseStockAsync(productA, 2),Times.Once);
            orderRepository.Verify(x => x.AddAsync(It.IsAny<Order>(), TestContext.Current.CancellationToken),Times.Never);
            unitOfWork.Verify(x => x.SaveChangesAsync(TestContext.Current.CancellationToken),Times.Never);
        }
        [Fact]
        public async Task Handle_ShouldNotIgnore_WhenStockCompensationFails()
        {
            // Arrange
            var productA = Guid.NewGuid();
            var productB = Guid.NewGuid();
            var command = new CreateOrderCommand
            {
                UserId = Guid.NewGuid(),
                Items = [new CreateOrderItemDto { ProductId = productA, Quantity = 2 }, new CreateOrderItemDto { ProductId = productB, Quantity = 3 }]
            };
            var orderRepository = new Mock<IOrderRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var productService = new Mock<IProductServiceClient>();

            productService.Setup(x => x.DecreaseStockAsync(productA, 2)).Returns(Task.CompletedTask);
            productService.Setup(x => x.DecreaseStockAsync(productB, 3)).ThrowsAsync(new HttpRequestException());
            productService.Setup(x => x.IncreaseStockAsync(productA, 2)).ThrowsAsync(new HttpRequestException());

            var handler = new CreateOrderCommandHandler(orderRepository.Object, productService.Object, unitOfWork.Object);

            // Act
            var action = () => handler.Handle(command, TestContext.Current.CancellationToken);

            // Assert
            await Assert.ThrowsAsync<HttpRequestException>(action);
        }
    }
}
