using Moq;
using OrderService.Application.Abstractions;
using OrderService.Application.Services;
using OrderService.Domain.Contracts;
using OrderService.Domain.Enums;
using OrderService.Domain.Orders;
using Xunit;

namespace OrderService.Application.Tests
{
    public class CancelExpiredOrdersServiceTests
    {
        private readonly Mock<IOrderRepository> orderRepositoryMock = new();
        private readonly Mock<IProductServiceClient> productServiceClientMock = new();
        private readonly Mock<IUnitOfWork> uowMock = new();

        [Fact]
        public async Task CancelExpiredAsync_ShouldRestoreStockAndCancelOrder()
        {
            // Arrange
            var productId = Guid.NewGuid();

            var order = new Order(Guid.NewGuid());

            order.AddItem(productId, 2);

            order.Confirm(
                new Dictionary<Guid, (decimal UnitPrice, CurrencyCode Currency)>
                {
                    [productId] = (100m, CurrencyCode.USD)
                },
                DateTime.UtcNow.AddDays(-4));

            orderRepositoryMock.Setup(x => x.GetConfirmedOrdersPastExpiryDateAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Order> { order });

            productServiceClientMock
                .Setup(x => x.IncreaseStockAsync(productId, 2, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new CancelExpiredOrdersService(
                orderRepositoryMock.Object,
                productServiceClientMock.Object,
                uowMock.Object);

            // Act
            await service.CancelExpiredAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order.Status);
            Assert.True(order.IsCancelledDueToExpiry);

            productServiceClientMock.Verify(
                x => x.IncreaseStockAsync(productId, 2, It.IsAny<CancellationToken>()), Times.Once);

            uowMock.Verify(x => x.SaveChangesAsync( It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CancelExpiredAsync_ShouldRestoreStockForAllItems()
        {
            // Arrange
            var productId1 = Guid.NewGuid();
            var productId2 = Guid.NewGuid();

            var order = new Order(Guid.NewGuid());

            order.AddItem(productId1, 2);
            order.AddItem(productId2, 5);

            order.Confirm(
                new Dictionary<Guid, (decimal UnitPrice, CurrencyCode Currency)>
                {
                    [productId1] = (100m, CurrencyCode.USD),
                    [productId2] = (50m, CurrencyCode.USD)
                },
                DateTime.UtcNow.AddDays(-4));

            orderRepositoryMock.Setup(x => x.GetConfirmedOrdersPastExpiryDateAsync(
                    It.IsAny<CancellationToken>())).ReturnsAsync(new List<Order> { order });

            productServiceClientMock
                .Setup(x => x.IncreaseStockAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new CancelExpiredOrdersService(orderRepositoryMock.Object, productServiceClientMock.Object, uowMock.Object);

            // Act
            await service.CancelExpiredAsync(TestContext.Current.CancellationToken);

            // Assert
            productServiceClientMock.Verify(
                x => x.IncreaseStockAsync(productId1, 2, It.IsAny<CancellationToken>()), Times.Once);

            productServiceClientMock.Verify(
                x => x.IncreaseStockAsync(productId2, 5, It.IsAny<CancellationToken>()), Times.Once);

            Assert.Equal(OrderStatus.Cancelled, order.Status);
            Assert.True(order.IsCancelledDueToExpiry);
        }

        [Fact]
        public async Task CancelExpiredAsync_ShouldDoNothing_WhenNoExpiredOrdersExist()
        {
            // Arrange
            orderRepositoryMock
                .Setup(x => x.GetConfirmedOrdersPastExpiryDateAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

            var service = new CancelExpiredOrdersService(
                orderRepositoryMock.Object,
                productServiceClientMock.Object,
                uowMock.Object);

            // Act
            await service.CancelExpiredAsync(TestContext.Current.CancellationToken);

            // Assert
            productServiceClientMock.Verify(
                x => x.IncreaseStockAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);

            uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CancelExpiredAsync_ShouldNotCancelOrder_WhenStockRestorationFails()
        {
            // Arrange
            var productId = Guid.NewGuid();

            var order = new Order(Guid.NewGuid());

            order.AddItem(productId, 2);

            order.Confirm(
                new Dictionary<Guid, (decimal UnitPrice, CurrencyCode Currency)>
                {
                    [productId] = (100m, CurrencyCode.USD)
                },
                DateTime.UtcNow.AddDays(-4));

            orderRepositoryMock.Setup(x => x.GetConfirmedOrdersPastExpiryDateAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Order> { order });

            productServiceClientMock
                .Setup(x => x.IncreaseStockAsync(productId, 2, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Product service unavailable"));

            var service = new CancelExpiredOrdersService(orderRepositoryMock.Object, productServiceClientMock.Object, uowMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.CancelExpiredAsync(TestContext.Current.CancellationToken));

            Assert.Equal(OrderStatus.Confirmed, order.Status);
            Assert.False(order.IsCancelledDueToExpiry);

            uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
