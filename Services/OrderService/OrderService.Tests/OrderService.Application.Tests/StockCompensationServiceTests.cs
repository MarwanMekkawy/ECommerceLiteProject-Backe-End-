using Moq;
using OrderService.Application.Abstractions.ClientsAbstractions;
using OrderService.Application.Services;
using OrderService.Domain.Contracts;
using OrderService.Domain.Enums;
using OrderService.Domain.Orders;
using Xunit;
using Microsoft.Extensions.Logging;

namespace OrderService.Application.Tests
{
    public class StockCompensationServiceTests
    {
        private readonly Mock<IOrderRepository> orderRepositoryMock = new();
        private readonly Mock<IProductServiceClient> productServiceClientMock = new();
        private readonly Mock<IUnitOfWork> uowMock = new();
        private readonly Mock<ILogger<StockCompensationService>> loggerMock = new();

        [Fact]
        public async Task CompensateAsync_ShouldRestoreStockAndMarkOrderAsCompensated()
        {
            // Arrange
            var productId = Guid.NewGuid();

            var order = new Order(Guid.NewGuid(), "example@gmail.com");

            order.AddItem(productId, 2);

            order.Confirm(
                new Dictionary<Guid, (string Name, decimal UnitPrice, CurrencyCode Currency)>
                {
                    [productId] = ("Test Product", 100m, CurrencyCode.USD)
                },
                DateTime.UtcNow.AddDays(-4));

            order.Expire();

            orderRepositoryMock.Setup(x => x.GetCancelledOrdersPendingStockCompensationAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Order> { order });

            productServiceClientMock.Setup(x => x.IncreaseStockAsync(productId, 2, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var service = new StockCompensationService(orderRepositoryMock.Object, productServiceClientMock.Object, uowMock.Object, loggerMock.Object);

            // Act
            await service.CompensateAsync(TestContext.Current.CancellationToken);

            // Assert
            productServiceClientMock.Verify(x => x.IncreaseStockAsync(productId, 2, It.IsAny<CancellationToken>()), Times.Once);

            Assert.True(order.IsStockCompensated);

            uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CompensateAsync_ShouldRestoreStockForAllItems()
        {
            // Arrange
            var productId1 = Guid.NewGuid();
            var productId2 = Guid.NewGuid();

            var order = new Order(Guid.NewGuid(), "example@gmail.com");

            order.AddItem(productId1, 2);
            order.AddItem(productId2, 5);

            order.Confirm(new Dictionary<Guid, (string Name, decimal UnitPrice, CurrencyCode Currency)>
                {
                    [productId1] = ("Test Product 1", 100m, CurrencyCode.USD),
                    [productId2] = ("Test Product 2", 50m, CurrencyCode.USD)
                },
                DateTime.UtcNow.AddDays(-4));

            order.Expire();

            orderRepositoryMock.Setup(x => x.GetCancelledOrdersPendingStockCompensationAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Order> { order });

            productServiceClientMock.Setup(x => x.IncreaseStockAsync(It.IsAny<Guid>(),It.IsAny<int>(),It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var service = new StockCompensationService(orderRepositoryMock.Object, productServiceClientMock.Object, uowMock.Object, loggerMock.Object);

            // Act
            await service.CompensateAsync(TestContext.Current.CancellationToken);

            // Assert
            productServiceClientMock.Verify(x => x.IncreaseStockAsync(productId1, 2, It.IsAny<CancellationToken>()), Times.Once);

            productServiceClientMock.Verify(x => x.IncreaseStockAsync(productId2, 5, It.IsAny<CancellationToken>()), Times.Once);

            Assert.True(order.IsStockCompensated);
        }

        [Fact]
        public async Task CompensateAsync_ShouldDoNothing_WhenNoOrdersRequireCompensation()
        {
            // Arrange
            orderRepositoryMock.Setup(x => x.GetCancelledOrdersPendingStockCompensationAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

            var service = new StockCompensationService(orderRepositoryMock.Object, productServiceClientMock.Object, uowMock.Object, loggerMock.Object);

            // Act
            await service.CompensateAsync(TestContext.Current.CancellationToken);

            // Assert
            productServiceClientMock.Verify(x => x.IncreaseStockAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);

            uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CompensateAsync_ShouldNotMarkOrderAsCompensated_WhenStockRestorationFails()
        {
            // Arrange
            var productId = Guid.NewGuid();

            var order = new Order(Guid.NewGuid(), "example@gmail.com");

            order.AddItem(productId, 2);

            order.Confirm(
                new Dictionary<Guid, (string Name, decimal UnitPrice, CurrencyCode Currency)>
                {
                    [productId] = ("Test Product", 100m, CurrencyCode.USD)
                },
                DateTime.UtcNow.AddDays(-4));

            order.Expire();

            orderRepositoryMock.Setup(x => x.GetCancelledOrdersPendingStockCompensationAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Order> { order });

            productServiceClientMock.Setup(x => x.IncreaseStockAsync(productId, 2, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Product service unavailable"));

            var service = new StockCompensationService(orderRepositoryMock.Object, productServiceClientMock.Object, uowMock.Object, loggerMock.Object);

            // Act
            await service.CompensateAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.False(order.IsStockCompensated);

            uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CompensateAsync_ShouldContinueWithOtherOrders_WhenOneOrderFails()
        {
            // Arrange
            var failedProductId = Guid.NewGuid();
            var successfulProductId = Guid.NewGuid();

            var failedOrder = new Order(Guid.NewGuid(), "failed@example.com");

            failedOrder.AddItem(failedProductId, 2);

            failedOrder.Confirm(
                new Dictionary<Guid, (string Name, decimal UnitPrice, CurrencyCode Currency)>
                {
                    [failedProductId] = ("Failed Product", 100m, CurrencyCode.USD)
                },
                DateTime.UtcNow.AddDays(-4));

            failedOrder.Expire();

            var successfulOrder = new Order(Guid.NewGuid(), "success@example.com");

            successfulOrder.AddItem(successfulProductId, 5);

            successfulOrder.Confirm(
                new Dictionary<Guid, (string Name, decimal UnitPrice, CurrencyCode Currency)>
                {
                    [successfulProductId] = ("Successful Product", 50m, CurrencyCode.USD)
                },
                DateTime.UtcNow.AddDays(-4));

            successfulOrder.Expire();

            orderRepositoryMock.Setup(x => x.GetCancelledOrdersPendingStockCompensationAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Order> { failedOrder, successfulOrder });

            productServiceClientMock.Setup(x => x.IncreaseStockAsync(failedProductId, 2, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Product service unavailable"));

            productServiceClientMock.Setup(x => x.IncreaseStockAsync(successfulProductId, 5, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var service = new StockCompensationService(orderRepositoryMock.Object, productServiceClientMock.Object, uowMock.Object, loggerMock.Object);

            // Act
            await service.CompensateAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.False(failedOrder.IsStockCompensated);
            Assert.True(successfulOrder.IsStockCompensated);

            productServiceClientMock.Verify(x => x.IncreaseStockAsync(failedProductId, 2, It.IsAny<CancellationToken>()), Times.Once);

            productServiceClientMock.Verify(x => x.IncreaseStockAsync(successfulProductId, 5, It.IsAny<CancellationToken>()), Times.Once);

            uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}