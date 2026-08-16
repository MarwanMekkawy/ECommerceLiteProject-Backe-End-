using Domain.Exceptions;
using Moq;
using OrderService.Application.Commands;
using OrderService.Domain.Contracts;
using OrderService.Domain.Exceptions.DomainExceptions;
using OrderService.Domain.Orders;
using Xunit;

namespace OrderService.Application.Tests
{
    public class IncreaseOrderItemCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldIncreaseItemQuantity_WhenOrderExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var order = new Order(userId);
            var productId = Guid.NewGuid();

            order.AddItem(productId, 2);

            var repository = new Mock<IOrderRepository>();

            repository.Setup(x => x.GetByIdAndUserIdTrackedAsync(order.Id, userId, It.IsAny<CancellationToken>())).ReturnsAsync(order);

            var uow = new Mock<IUnitOfWork>();

            var handler = new IncreaseOrderItemCommandHandler(repository.Object, uow.Object);

            var command = new IncreaseOrderItemCommand(userId, order.Id, productId, 3);

            // Act
            await handler.HandleAsync(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(5, order.Items.First().Quantity);

            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task Handle_ShouldThrow_WhenOrderDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var repository = new Mock<IOrderRepository>();

            repository.Setup(x => x.GetByIdAndUserIdTrackedAsync(orderId, userId, It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);

            var uow = new Mock<IUnitOfWork>();

            var handler = new IncreaseOrderItemCommandHandler(repository.Object, uow.Object);

            var command = new IncreaseOrderItemCommand(userId, orderId, productId, 1);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));

            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        public async Task Handle_ShouldThrow_WhenProductDoesNotExistInOrder()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var order = new Order(userId);

            var existingProductId = Guid.NewGuid();
            var requestedProductId = Guid.NewGuid();

            order.AddItem(existingProductId, 2);

            var repository = new Mock<IOrderRepository>();

            repository.Setup(x => x.GetByIdAndUserIdTrackedAsync(order.Id, userId, It.IsAny<CancellationToken>())).ReturnsAsync(order);

            var uow = new Mock<IUnitOfWork>();

            var handler = new IncreaseOrderItemCommandHandler(repository.Object, uow.Object);

            var command = new IncreaseOrderItemCommand(userId, order.Id, requestedProductId, 1);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOrderItemException>(() => handler.HandleAsync(command, TestContext.Current.CancellationToken));

            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        public async Task Handle_ShouldPropagateDomainException_WhenQuantityIsInvalid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var order = new Order(userId);
            var productId = Guid.NewGuid();

            order.AddItem(productId, 2);

            var repository = new Mock<IOrderRepository>();

            repository.Setup(x => x.GetByIdAndUserIdTrackedAsync(order.Id, userId, It.IsAny<CancellationToken>())).ReturnsAsync(order);

            var uow = new Mock<IUnitOfWork>();

            var handler = new IncreaseOrderItemCommandHandler(repository.Object, uow.Object);

            var command = new IncreaseOrderItemCommand(userId, order.Id, productId, 0);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOrderItemException>(() =>handler.HandleAsync(command, TestContext.Current.CancellationToken));

            Assert.Equal(2, order.Items.First().Quantity);

            uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
