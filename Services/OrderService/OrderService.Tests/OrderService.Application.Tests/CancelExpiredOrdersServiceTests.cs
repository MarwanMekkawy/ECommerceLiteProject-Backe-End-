using Moq;
using OrderService.Application.Abstractions.ClientsAbstractions;
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
        private readonly Mock<INotificationServiceClient> notificationServiceClientMock = new();
        private readonly Mock<IUnitOfWork> uowMock = new();

        [Fact]
        public async Task CancelExpiredAsync_ShouldCancelExpiredOrder()
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

            orderRepositoryMock.Setup(x => x.GetConfirmedOrdersPastExpiryDateAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Order> { order });

            var service = new CancelExpiredOrdersService(orderRepositoryMock.Object, notificationServiceClientMock.Object, uowMock.Object);

            // Act
            await service.CancelExpiredAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order.Status);
            Assert.True(order.IsCancelledDueToExpiry);

            uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CancelExpiredAsync_ShouldCancelAllExpiredOrders()
        {
            // Arrange
            var productId1 = Guid.NewGuid();
            var productId2 = Guid.NewGuid();

            var order1 = new Order(Guid.NewGuid(), "example@gmail.com");
            var order2 = new Order(Guid.NewGuid(), "example@gmail.com");

            order1.AddItem(productId1, 2);
            order2.AddItem(productId2, 5);

            order1.Confirm(
                new Dictionary<Guid, (string Name, decimal UnitPrice, CurrencyCode Currency)>
                {
                    [productId1] = ("Test Product 1", 100m, CurrencyCode.USD)
                },
                DateTime.UtcNow.AddDays(-4));

            order2.Confirm(
                new Dictionary<Guid, (string Name, decimal UnitPrice, CurrencyCode Currency)>
                {
                    [productId2] = ("Test Product 2", 50m, CurrencyCode.USD)
                },
                DateTime.UtcNow.AddDays(-5));

            orderRepositoryMock.Setup(x => x.GetConfirmedOrdersPastExpiryDateAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Order> { order1, order2 });

            var service = new CancelExpiredOrdersService(orderRepositoryMock.Object, notificationServiceClientMock.Object, uowMock.Object);

            // Act
            await service.CancelExpiredAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order1.Status);
            Assert.True(order1.IsCancelledDueToExpiry);

            Assert.Equal(OrderStatus.Cancelled, order2.Status);
            Assert.True(order2.IsCancelledDueToExpiry);

            uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CancelExpiredAsync_ShouldSendCancellationNotification()
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

            orderRepositoryMock.Setup(x => x.GetConfirmedOrdersPastExpiryDateAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Order> { order });

            var service = new CancelExpiredOrdersService(orderRepositoryMock.Object, notificationServiceClientMock.Object, uowMock.Object);

            // Act
            await service.CancelExpiredAsync(TestContext.Current.CancellationToken);

            // Assert
            notificationServiceClientMock.Verify(
                x => x.SendOrderCancelledDueExpirationAsync(
                    order.UserId,
                    order.CustomerEmail,
                    "Customer",
                    order.Id,
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CancelExpiredAsync_ShouldDoNothing_WhenNoExpiredOrdersExist()
        {
            // Arrange
            orderRepositoryMock
                .Setup(x => x.GetConfirmedOrdersPastExpiryDateAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

            var service = new CancelExpiredOrdersService(orderRepositoryMock.Object, notificationServiceClientMock.Object, uowMock.Object);

            // Act
            await service.CancelExpiredAsync(TestContext.Current.CancellationToken);

            // Assert
            notificationServiceClientMock.Verify(
                x => x.SendOrderCancelledDueExpirationAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()), Times.Never);

            uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
