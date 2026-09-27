using Domain.Exceptions;
using Moq;
using OrderService.Application.Abstractions.ClientsAbstractions;
using OrderService.Application.Commands;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;
using Xunit;

namespace OrderService.Application.Tests
{
    public class CancelOrderInternalCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldCancelOrder_WhenOrderExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var email = "example@gmail.com";

            var order = new Order(userId, email);

            var repository = new Mock<IOrderRepository>();
            var notificationServiceClientMock = new Mock<INotificationServiceClient>();
            var uow = new Mock<IUnitOfWork>();

            repository
                .Setup(x => x.GetByIdTrackedAsync(
                    orderId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var handler = new CancelRefundedOrderInternalCommandHandler(repository.Object, notificationServiceClientMock.Object, uow.Object);

            var command = new CancelRefundedOrderInternalCommand(orderId);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(OrderStatus.CancelledAndRefunded, order.Status);

            repository.Verify(x => x.GetByIdTrackedAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);

            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            var repository = new Mock<IOrderRepository>();
            var notificationServiceClientMock = new Mock<INotificationServiceClient>();
            var uow = new Mock<IUnitOfWork>();

            repository
                .Setup(x => x.GetByIdTrackedAsync(
                    orderId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            var handler = new CancelRefundedOrderInternalCommandHandler(repository.Object, notificationServiceClientMock.Object, uow.Object);

            var command = new CancelRefundedOrderInternalCommand(orderId);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));

            repository.Verify(x => x.GetByIdTrackedAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);

            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
