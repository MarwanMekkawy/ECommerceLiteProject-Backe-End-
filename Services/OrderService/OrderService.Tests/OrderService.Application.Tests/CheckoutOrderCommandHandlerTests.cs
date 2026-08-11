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
        public async Task Handle_ShouldReturnCheckoutDetails_WhenCommandIsValid()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var productId = Guid.NewGuid();
            order.AddItem(productId, 2);
            var product = new ProductDto
            {
                ProductId = productId,
                Price = 10,
                Currency = CurrencyCode.USD
            };
            var command = new CheckoutOrderCommand(order.Id);
            var orderRepository = new Mock<IOrderRepository>();
            orderRepository.Setup(x => x.GetByIdUntrackedAsync(order.Id, TestContext.Current.CancellationToken)).ReturnsAsync(order);
            var productService = new Mock<IProductServiceClient>();
            productService.Setup(x => x.GetProductForCheckoutAsync(productId, TestContext.Current.CancellationToken)).ReturnsAsync(product);
            var handler = new CheckoutOrderCommandHandler(orderRepository.Object, productService.Object);

            // Act
            var result = await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(order.Id, result.OrderId);
            Assert.Single(result.Items);
            Assert.Equal(productId, result.Items.First().ProductId);
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
            var command = new CheckoutOrderCommand(orderId);
            var orderRepository = new Mock<IOrderRepository>();
            orderRepository.Setup(x => x.GetByIdUntrackedAsync(orderId, TestContext.Current.CancellationToken)).ReturnsAsync((Order?)null);
            var productService = new Mock<IProductServiceClient>();
            var handler = new CheckoutOrderCommandHandler(orderRepository.Object, productService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));
            productService.Verify(x => x.GetProductForCheckoutAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            productService.Verify(x => x.DecreaseStockAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldGetProductPrice_WhenCheckoutIsValid()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var productId = Guid.NewGuid();
            order.AddItem(productId, 2);
            var product = new ProductDto
            {
                ProductId = productId,
                Price = 15,
                Currency = CurrencyCode.USD
            };
            var command = new CheckoutOrderCommand(order.Id);
            var orderRepository = new Mock<IOrderRepository>();
            orderRepository.Setup(x => x.GetByIdUntrackedAsync(order.Id, TestContext.Current.CancellationToken)).ReturnsAsync(order);
            var productService = new Mock<IProductServiceClient>();
            productService.Setup(x => x.GetProductForCheckoutAsync(productId, TestContext.Current.CancellationToken)).ReturnsAsync(product);
            var handler = new CheckoutOrderCommandHandler(orderRepository.Object, productService.Object);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            productService.Verify(x => x.GetProductForCheckoutAsync(productId, TestContext.Current.CancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldDecreaseStock_WhenCheckoutIsValid()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var productId = Guid.NewGuid();
            order.AddItem(productId, 2);
            var product = new ProductDto
            {
                ProductId = productId,
                Price = 10,
                Currency = CurrencyCode.USD
            };
            var command = new CheckoutOrderCommand(order.Id);
            var orderRepository = new Mock<IOrderRepository>();
            orderRepository.Setup(x => x.GetByIdUntrackedAsync(order.Id, TestContext.Current.CancellationToken)).ReturnsAsync(order);
            var productService = new Mock<IProductServiceClient>();
            productService.Setup(x => x.GetProductForCheckoutAsync(productId, TestContext.Current.CancellationToken)).ReturnsAsync(product);
            var handler = new CheckoutOrderCommandHandler(orderRepository.Object, productService.Object);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            productService.Verify(x => x.DecreaseStockAsync(productId, 2, TestContext.Current.CancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCalculateCorrectTotal_WhenOrderHasMultipleItems()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var productA = Guid.NewGuid();
            var productB = Guid.NewGuid();
            order.AddItem(productA, 2);
            order.AddItem(productB, 3);
            var command = new CheckoutOrderCommand(order.Id);
            var orderRepository = new Mock<IOrderRepository>();
            orderRepository.Setup(x => x.GetByIdUntrackedAsync(order.Id, TestContext.Current.CancellationToken)).ReturnsAsync(order);
            var productService = new Mock<IProductServiceClient>();
            productService.Setup(x => x.GetProductForCheckoutAsync(productA, TestContext.Current.CancellationToken)).ReturnsAsync(new ProductDto
            {
                ProductId = productA,
                Price = 10,
                Currency = CurrencyCode.USD
            });
            productService.Setup(x => x.GetProductForCheckoutAsync(productB, TestContext.Current.CancellationToken)).ReturnsAsync(new ProductDto
            {
                ProductId = productB,
                Price = 5,
                Currency = CurrencyCode.USD
            });
            var handler = new CheckoutOrderCommandHandler(orderRepository.Object, productService.Object);

            // Act
            var result = await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(35, result.Total);
            Assert.Equal(20, result.Items.First(x => x.ProductId == productA).Total);
            Assert.Equal(15, result.Items.First(x => x.ProductId == productB).Total);
        }

        [Fact]
        public async Task Handle_ShouldRestorePreviousStock_WhenLaterStockDecreaseFails()
        {
            // Arrange
            var productA = Guid.NewGuid();
            var productB = Guid.NewGuid();
            var order = new Order(Guid.NewGuid());
            order.AddItem(productA, 2);
            order.AddItem(productB, 3);
            var command = new CheckoutOrderCommand(order.Id);
            var orderRepository = new Mock<IOrderRepository>();
            orderRepository.Setup(x => x.GetByIdUntrackedAsync(order.Id, TestContext.Current.CancellationToken)).ReturnsAsync(order);
            var productService = new Mock<IProductServiceClient>();
            productService.Setup(x => x.GetProductForCheckoutAsync(productA, TestContext.Current.CancellationToken)).ReturnsAsync(new ProductDto
            {
                ProductId = productA,
                Price = 10,
                Currency = CurrencyCode.USD
            });
            productService.Setup(x => x.GetProductForCheckoutAsync(productB, TestContext.Current.CancellationToken)).ReturnsAsync(new ProductDto
            {
                ProductId = productB,
                Price = 5,
                Currency = CurrencyCode.USD
            });
            productService.Setup(x => x.DecreaseStockAsync(productA, 2, TestContext.Current.CancellationToken)).Returns(Task.CompletedTask);
            productService.Setup(x => x.DecreaseStockAsync(productB, 3, TestContext.Current.CancellationToken)).ThrowsAsync(new HttpRequestException());
            var handler = new CheckoutOrderCommandHandler(orderRepository.Object, productService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));
            productService.Verify(x => x.IncreaseStockAsync(productA, 2, TestContext.Current.CancellationToken), Times.Once);
            productService.Verify(x => x.IncreaseStockAsync(productB, 3, TestContext.Current.CancellationToken), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldRestoreStock_WhenPriceLookupFails()
        {
            // Arrange
            var productA = Guid.NewGuid();
            var productB = Guid.NewGuid();
            var order = new Order(Guid.NewGuid());
            order.AddItem(productA, 2);
            order.AddItem(productB, 3);
            var command = new CheckoutOrderCommand(order.Id);
            var orderRepository = new Mock<IOrderRepository>();
            orderRepository.Setup(x => x.GetByIdUntrackedAsync(order.Id, TestContext.Current.CancellationToken)).ReturnsAsync(order);
            var productService = new Mock<IProductServiceClient>();
            productService.Setup(x => x.GetProductForCheckoutAsync(productA, TestContext.Current.CancellationToken)).ReturnsAsync(new ProductDto
            {
                ProductId = productA,
                Price = 10,
                Currency = CurrencyCode.USD
            });
            productService.Setup(x => x.GetProductForCheckoutAsync(productB, TestContext.Current.CancellationToken)).ThrowsAsync(new HttpRequestException());
            productService.Setup(x => x.DecreaseStockAsync(productA, 2, TestContext.Current.CancellationToken)).Returns(Task.CompletedTask);
            var handler = new CheckoutOrderCommandHandler(orderRepository.Object, productService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));
            productService.Verify(x => x.IncreaseStockAsync(productA, 2, TestContext.Current.CancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldNotRestoreStock_WhenCheckoutSucceeds()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var productId = Guid.NewGuid();
            order.AddItem(productId, 2);
            var product = new ProductDto
            {
                ProductId = productId,
                Price = 10,
                Currency = CurrencyCode.USD
            };
            var command = new CheckoutOrderCommand(order.Id);
            var orderRepository = new Mock<IOrderRepository>();
            orderRepository.Setup(x => x.GetByIdUntrackedAsync(order.Id, TestContext.Current.CancellationToken)).ReturnsAsync(order);
            var productService = new Mock<IProductServiceClient>();
            productService.Setup(x => x.GetProductForCheckoutAsync(productId, TestContext.Current.CancellationToken)).ReturnsAsync(product);
            var handler = new CheckoutOrderCommandHandler(orderRepository.Object, productService.Object);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            productService.Verify(x => x.IncreaseStockAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenProductsHaveDifferentCurrencies()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var productA = Guid.NewGuid();
            var productB = Guid.NewGuid();
            order.AddItem(productA, 1);
            order.AddItem(productB, 1);
            var command = new CheckoutOrderCommand(order.Id);
            var orderRepository = new Mock<IOrderRepository>();
            orderRepository.Setup(x => x.GetByIdUntrackedAsync(order.Id, TestContext.Current.CancellationToken)).ReturnsAsync(order);
            var productService = new Mock<IProductServiceClient>();
            productService.Setup(x => x.GetProductForCheckoutAsync(productA, TestContext.Current.CancellationToken)).ReturnsAsync(new ProductDto
            {
                ProductId = productA,
                Price = 10,
                Currency = CurrencyCode.USD
            });
            productService.Setup(x => x.GetProductForCheckoutAsync(productB, TestContext.Current.CancellationToken)).ReturnsAsync(new ProductDto
            {
                ProductId = productB,
                Price = 10,
                Currency = CurrencyCode.EUR
            });
            var handler = new CheckoutOrderCommandHandler(orderRepository.Object, productService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOrderException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));
        }
    }
}
