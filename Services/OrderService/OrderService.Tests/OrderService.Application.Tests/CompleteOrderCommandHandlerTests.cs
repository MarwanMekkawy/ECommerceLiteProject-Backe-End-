using Domain.Exceptions;
using Moq;
using OrderService.Application.Abstractions.ClientsAbstractions;
using OrderService.Application.Commands;
using OrderService.Domain.Contracts;
using OrderService.Domain.Enums;
using OrderService.Domain.Orders;
using Xunit;

namespace OrderService.Application.Tests
{
    public class CompleteOrderCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldCompleteOrder_WhenOrderIsConfirmed()
        {
            // Arrange
            var order = new Order(Guid.NewGuid(), "example@gmail.com");
            var productId = Guid.NewGuid();
            order.AddItem(productId, 1);

            var productPrices = new Dictionary<Guid, (string Name, decimal UnitPrice, CurrencyCode Currency)>
            {
                [productId] = ("Test Product", 10, CurrencyCode.USD)
            };

            order.Confirm(productPrices, DateTime.UtcNow);

            var repository = new Mock<IOrderRepository>();
            repository.Setup(x => x.GetByIdTrackedAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync(order);

            var notificationServiceClientMock = new Mock<INotificationServiceClient>();
            var uow = new Mock<IUnitOfWork>();
            var handler = new CompleteOrderInternalCommandHandler(repository.Object, notificationServiceClientMock.Object, uow.Object);
            var command = new CompleteOrderInternalCommand(order.Id);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(OrderStatus.Completed, order.Status);
            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact]
        public async Task Handle_ShouldThrow_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var repository = new Mock<IOrderRepository>();
            repository.Setup(x => x.GetByIdTrackedAsync(orderId, It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);
            var notificationServiceClientMock = new Mock<INotificationServiceClient>();
            var uow = new Mock<IUnitOfWork>();
            var handler = new CompleteOrderInternalCommandHandler(repository.Object, notificationServiceClientMock.Object, uow.Object);
            var command = new CompleteOrderInternalCommand(orderId);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));
            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
        [Fact]
        public async Task Handle_ShouldThrowConflictException_WhenOrderIsCancelledDueToExpiry()
        {
            // Arrange
            var order = new Order(Guid.NewGuid(), "example@gmail.com");
            order.AddItem(Guid.NewGuid(), 1);

            var productPrices = new Dictionary<Guid, (string Name, decimal UnitPrice, CurrencyCode Currency)>
            {
                [order.Items.First().ProductId] = ("Test Product", 10, CurrencyCode.USD)
            };

            order.Confirm(productPrices, DateTime.UtcNow);
            order.Expire();

            var repository = new Mock<IOrderRepository>();
            repository.Setup(x => x.GetByIdTrackedAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync(order);
            var notificationServiceClientMock = new Mock<INotificationServiceClient>();
            var uow = new Mock<IUnitOfWork>();
            var handler = new CompleteOrderInternalCommandHandler(repository.Object, notificationServiceClientMock.Object, uow.Object);
            var command = new CompleteOrderInternalCommand(order.Id);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));
            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
