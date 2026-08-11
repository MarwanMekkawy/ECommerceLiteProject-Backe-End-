using Domain.Exceptions;
using Moq;
using OrderService.Application.Abstractions;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Domain.Contracts;
using OrderService.Domain.Enums;
using OrderService.Domain.Exceptions.DomainExceptions;
using OrderService.Domain.Orders;
using Xunit;

namespace OrderService.Application.Tests
{
    public class CheckoutOrderCommandHandlerTests
        {
        [Fact]
        public async Task Handle_ShouldConfirmOrderAndReturnCheckoutDetails_WhenCheckoutIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var order = new Order(userId);
            var productId = Guid.NewGuid();
            order.AddItem(productId, 2);

            var product = new ProductDto
            {
                ProductId = productId,
                Price = 10,
                Currency = CurrencyCode.USD
            };

            var command = new CheckoutOrderCommand(userId, order.Id);

            var orderRepository = new Mock<IOrderRepository>();
            orderRepository.Setup(x => x.GetByIdAndUserIdTrackedAsync(order.Id, userId, TestContext.Current.CancellationToken)).ReturnsAsync(order);

            var productServiceClient = new Mock<IProductServiceClient>();
            productServiceClient.Setup(x => x.GetProductForCheckoutAsync(productId, TestContext.Current.CancellationToken)).ReturnsAsync(product);

            var uow = new Mock<IUnitOfWork>();

            var handler = new CheckoutOrderCommandHandler(orderRepository.Object, productServiceClient.Object, uow.Object);

            // Act
            var result = await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(order.Id, result.OrderId);
            Assert.Equal(OrderStatus.Confirmed, order.Status);
            Assert.Equal(20, order.Total);
            Assert.Equal(CurrencyCode.USD, order.Currency);
            Assert.Equal(2, result.Items.First().Quantity);
            Assert.Equal(10, result.Items.First().UnitPrice);
            Assert.Equal(20, result.Items.First().Total);
            Assert.Equal(20, result.Total);
            Assert.Equal(CurrencyCode.USD, result.Currency);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new CheckoutOrderCommand(userId, orderId);

            var orderRepository = new Mock<IOrderRepository>();
            orderRepository.Setup(x => x.GetByIdAndUserIdTrackedAsync(orderId, userId, TestContext.Current.CancellationToken)).ReturnsAsync((Order?)null);

            var productServiceClient = new Mock<IProductServiceClient>();
            var uow = new Mock<IUnitOfWork>();

            var handler = new CheckoutOrderCommandHandler(orderRepository.Object, productServiceClient.Object, uow.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenOrderIsNotPending()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var order = new Order(userId);
            order.Cancel();

            var command = new CheckoutOrderCommand(userId, order.Id);

            var orderRepository = new Mock<IOrderRepository>();
            orderRepository.Setup(x => x.GetByIdAndUserIdTrackedAsync(order.Id, userId, TestContext.Current.CancellationToken)).ReturnsAsync(order);

            var productServiceClient = new Mock<IProductServiceClient>();
            var uow = new Mock<IUnitOfWork>();

            var handler = new CheckoutOrderCommandHandler(orderRepository.Object, productServiceClient.Object, uow.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOrderException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenProductDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var order = new Order(userId);
            var productId = Guid.NewGuid();
            order.AddItem(productId, 2);

            var command = new CheckoutOrderCommand(userId, order.Id);

            var orderRepository = new Mock<IOrderRepository>();
            orderRepository.Setup(x => x.GetByIdAndUserIdTrackedAsync(order.Id, userId, TestContext.Current.CancellationToken)).ReturnsAsync(order);

            var productServiceClient = new Mock<IProductServiceClient>();
            productServiceClient.Setup(x => x.GetProductForCheckoutAsync(productId, TestContext.Current.CancellationToken)).ReturnsAsync((ProductDto)null!);

            var uow = new Mock<IUnitOfWork>();

            var handler = new CheckoutOrderCommandHandler(orderRepository.Object, productServiceClient.Object, uow.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task Handle_ShouldDecreaseStock_WhenCheckoutIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var order = new Order(userId);
            var productId = Guid.NewGuid();
            order.AddItem(productId, 2);

            var product = new ProductDto
            {
                ProductId = productId,
                Price = 10,
                Currency = CurrencyCode.USD
            };

            var command = new CheckoutOrderCommand(userId, order.Id);

            var orderRepository = new Mock<IOrderRepository>();
            orderRepository.Setup(x => x.GetByIdAndUserIdTrackedAsync(order.Id, userId, TestContext.Current.CancellationToken)).ReturnsAsync(order);

            var productServiceClient = new Mock<IProductServiceClient>();
            productServiceClient.Setup(x => x.GetProductForCheckoutAsync(productId, TestContext.Current.CancellationToken)).ReturnsAsync(product);

            var uow = new Mock<IUnitOfWork>();

            var handler = new CheckoutOrderCommandHandler(orderRepository.Object, productServiceClient.Object, uow.Object);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            productServiceClient.Verify(x => x.DecreaseStockAsync(productId, 2, TestContext.Current.CancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldSnapshotProductPrice_WhenCheckoutIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var order = new Order(userId);
            var productId = Guid.NewGuid();
            order.AddItem(productId, 2);

            var product = new ProductDto
            {
                ProductId = productId,
                Price = 25,
                Currency = CurrencyCode.USD
            };

            var command = new CheckoutOrderCommand(userId, order.Id);

            var orderRepository = new Mock<IOrderRepository>();
            orderRepository.Setup(x => x.GetByIdAndUserIdTrackedAsync(order.Id, userId, TestContext.Current.CancellationToken)).ReturnsAsync(order);

            var productServiceClient = new Mock<IProductServiceClient>();
            productServiceClient.Setup(x => x.GetProductForCheckoutAsync(productId, TestContext.Current.CancellationToken)).ReturnsAsync(product);

            var uow = new Mock<IUnitOfWork>();

            var handler = new CheckoutOrderCommandHandler(orderRepository.Object, productServiceClient.Object, uow.Object);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            var item = order.Items.First();

            Assert.Equal(25, item.UnitPrice);
            Assert.Equal(CurrencyCode.USD, item.Currency);
            Assert.Equal(50, item.Total);
        }

        [Fact]
        public async Task Handle_ShouldSetPaymentExpiration_WhenCheckoutIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var order = new Order(userId);
            var productId = Guid.NewGuid();
            order.AddItem(productId, 2);

            var product = new ProductDto
            {
                ProductId = productId,
                Price = 10,
                Currency = CurrencyCode.USD
            };

            var command = new CheckoutOrderCommand(userId, order.Id);

            var orderRepository = new Mock<IOrderRepository>();
            orderRepository.Setup(x => x.GetByIdAndUserIdTrackedAsync(order.Id, userId, TestContext.Current.CancellationToken)).ReturnsAsync(order);

            var productServiceClient = new Mock<IProductServiceClient>();
            productServiceClient.Setup(x => x.GetProductForCheckoutAsync(productId, TestContext.Current.CancellationToken)).ReturnsAsync(product);

            var uow = new Mock<IUnitOfWork>();

            var handler = new CheckoutOrderCommandHandler(orderRepository.Object, productServiceClient.Object, uow.Object);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(order.ConfirmedAt);
            Assert.NotNull(order.PaymentExpiresAt);
            Assert.Equal(3, (order.PaymentExpiresAt.Value - order.ConfirmedAt.Value).TotalDays);
        }

        [Fact]
        public async Task Handle_ShouldSaveChanges_WhenCheckoutIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var order = new Order(userId);
            var productId = Guid.NewGuid();
            order.AddItem(productId, 2);

            var product = new ProductDto
            {
                ProductId = productId,
                Price = 10,
                Currency = CurrencyCode.USD
            };

            var command = new CheckoutOrderCommand(userId, order.Id);

            var orderRepository = new Mock<IOrderRepository>();
            orderRepository.Setup(x => x.GetByIdAndUserIdTrackedAsync(order.Id, userId, TestContext.Current.CancellationToken)).ReturnsAsync(order);

            var productServiceClient = new Mock<IProductServiceClient>();
            productServiceClient.Setup(x => x.GetProductForCheckoutAsync(productId, TestContext.Current.CancellationToken)).ReturnsAsync(product);

            var uow = new Mock<IUnitOfWork>();

            var handler = new CheckoutOrderCommandHandler(orderRepository.Object, productServiceClient.Object, uow.Object);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            uow.Verify(x => x.SaveChangesAsync(TestContext.Current.CancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldRestoreStock_WhenCheckoutFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var order = new Order(userId);
            var productId = Guid.NewGuid();
            order.AddItem(productId, 2);

            var product = new ProductDto
            {
                ProductId = productId,
                Price = 10,
                Currency = CurrencyCode.USD
            };

            var command = new CheckoutOrderCommand(userId, order.Id);

            var orderRepository = new Mock<IOrderRepository>();
            orderRepository.Setup(x => x.GetByIdAndUserIdTrackedAsync(order.Id, userId, TestContext.Current.CancellationToken)).ReturnsAsync(order);

            var productServiceClient = new Mock<IProductServiceClient>();
            productServiceClient.Setup(x => x.GetProductForCheckoutAsync(productId, TestContext.Current.CancellationToken)).ReturnsAsync(product);
            productServiceClient.Setup(x => x.DecreaseStockAsync(productId, 2, TestContext.Current.CancellationToken)).ThrowsAsync(new HttpRequestException());

            var uow = new Mock<IUnitOfWork>();

            var handler = new CheckoutOrderCommandHandler(orderRepository.Object, productServiceClient.Object, uow.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));

            productServiceClient.Verify(x => x.IncreaseStockAsync(productId, 2, TestContext.Current.CancellationToken), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldRestorePreviousStock_WhenLaterStockDecreaseFails()
        {
            // Arrange
            var productA = Guid.NewGuid();
            var productB = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var order = new Order(userId);
            order.AddItem(productA, 2);
            order.AddItem(productB, 3);

            var command = new CheckoutOrderCommand(userId, order.Id);

            var orderRepository = new Mock<IOrderRepository>();
            orderRepository.Setup(x => x.GetByIdAndUserIdTrackedAsync(order.Id, userId, TestContext.Current.CancellationToken)).ReturnsAsync(order);

            var productServiceClient = new Mock<IProductServiceClient>();
            productServiceClient.Setup(x => x.GetProductForCheckoutAsync(productA, TestContext.Current.CancellationToken)).ReturnsAsync(new ProductDto
            {
                ProductId = productA,
                Price = 10,
                Currency = CurrencyCode.USD
            });
            productServiceClient.Setup(x => x.GetProductForCheckoutAsync(productB, TestContext.Current.CancellationToken)).ReturnsAsync(new ProductDto
            {
                ProductId = productB,
                Price = 20,
                Currency = CurrencyCode.USD
            });

            productServiceClient.Setup(x => x.DecreaseStockAsync(productA, 2, TestContext.Current.CancellationToken)).Returns(Task.CompletedTask);
            productServiceClient.Setup(x => x.DecreaseStockAsync(productB, 3, TestContext.Current.CancellationToken)).ThrowsAsync(new HttpRequestException());

            var uow = new Mock<IUnitOfWork>();

            var handler = new CheckoutOrderCommandHandler(orderRepository.Object, productServiceClient.Object, uow.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));

            productServiceClient.Verify(x => x.IncreaseStockAsync(productA, 2, TestContext.Current.CancellationToken), Times.Once);
            productServiceClient.Verify(x => x.IncreaseStockAsync(productB, 3, TestContext.Current.CancellationToken), Times.Never);
        }
    }
}
