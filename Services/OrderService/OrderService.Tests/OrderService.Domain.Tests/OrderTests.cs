using OrderService.Domain.Enums;
using OrderService.Domain.Exceptions.DomainExceptions;
using OrderService.Domain.Orders;
using Xunit;

namespace OrderService.Domain.Tests
{
    public class OrderTests
    {
        [Fact]
        public void Constructor_ShouldThrow_WhenUserIdIsEmpty()
        {
            var userId = Guid.Empty;

            var action = () => new Order(userId);

            Assert.Throws<InvalidOrderException>(action);
        }

        [Fact]
        public void AddItem_ShouldThrow_WhenProductIdIsEmpty()
        {
            var order = new Order(Guid.NewGuid());

            var action = () => order.AddItem(Guid.Empty, 1);

            Assert.Throws<InvalidOrderItemException>(action);
        }

        [Fact]
        public void AddItem_ShouldIncreaseQuantity_WhenItemExists()
        {
            var order = new Order(Guid.NewGuid());
            var productId = Guid.NewGuid();
            order.AddItem(productId, 2);

            order.AddItem(productId, 3);

            var item = Assert.Single(order.Items);
            Assert.Equal(productId, item.ProductId);
            Assert.Equal(5, item.Quantity);
        }

        [Fact]
        public void AddItem_ShouldThrow_WhenQuantityIsZero()
        {
            var order = new Order(Guid.NewGuid());
            var productId = Guid.NewGuid();

            var action = () => order.AddItem(productId, 0);

            Assert.Throws<InvalidOrderItemException>(action);
        }

        [Fact]
        public void AddItem_ShouldThrow_WhenQuantityIsNegative()
        {
            var order = new Order(Guid.NewGuid());
            var productId = Guid.NewGuid();

            var action = () => order.AddItem(productId, -1);

            Assert.Throws<InvalidOrderItemException>(action);
        }

        [Fact]
        public void CreateOrder_ShouldCreatePendingOrder_WhenUserIdIsValid()
        {
            var userId = Guid.NewGuid();

            var order = new Order(userId);

            Assert.Equal(userId, order.UserId);
            Assert.Equal(OrderStatus.Pending, order.Status);
            Assert.NotEqual(Guid.Empty, order.Id);
        }

        [Fact]
        public void AddItem_ShouldAddItem_WhenProductDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var order = new Order(userId);
            order.AddItem(productId, 2);

            Assert.Equal(1, order.Items.Count(x => x.ProductId == productId));
            Assert.Equal(2, order.Items.Single().Quantity);
        }

        [Fact]
        public void Confirm_ShouldChangeStatusToConfirmed_WhenOrderIsPending()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var productId = Guid.NewGuid();
            order.AddItem(productId, 2);

            var productPrices = new Dictionary<Guid, (decimal UnitPrice, CurrencyCode Currency)>
            {
                [productId] = (10, CurrencyCode.USD)
            };

            // Act
            order.Confirm(productPrices, DateTime.UtcNow);

            // Assert
            Assert.Equal(OrderStatus.Confirmed, order.Status);
            Assert.Equal(20, order.Total);
            Assert.Equal(CurrencyCode.USD, order.Currency);
            Assert.NotNull(order.ConfirmedAt);
            Assert.NotNull(order.PaymentExpiresAt);
        }

        [Fact]
        public void Confirm_ShouldSnapshotItemPrice_WhenOrderIsPending()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var productId = Guid.NewGuid();
            order.AddItem(productId, 2);

            var productPrices = new Dictionary<Guid, (decimal UnitPrice, CurrencyCode Currency)>
            {
                [productId] = (15, CurrencyCode.USD)
            };

            // Act
            order.Confirm(productPrices, DateTime.UtcNow);

            // Assert
            var item = order.Items.Single();
            Assert.Equal(15, item.UnitPrice);
            Assert.Equal(CurrencyCode.USD, item.Currency);
            Assert.Equal(30, item.Total);
        }

        [Fact]
        public void Confirm_ShouldSetPaymentExpirationToThreeDays_WhenOrderIsPending()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var productId = Guid.NewGuid();
            order.AddItem(productId, 1);

            var productPrices = new Dictionary<Guid, (decimal UnitPrice, CurrencyCode Currency)>
            {
                [productId] = (10, CurrencyCode.USD)
            };

            // Act
            order.Confirm(productPrices, DateTime.UtcNow);

            // Assert
            Assert.NotNull(order.ConfirmedAt);
            Assert.NotNull(order.PaymentExpiresAt);
            Assert.Equal(3, (order.PaymentExpiresAt!.Value - order.ConfirmedAt!.Value).TotalDays);
        }

        [Fact]
        public void Confirm_ShouldThrow_WhenOrderHasNoItems()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var productPrices = new Dictionary<Guid, (decimal UnitPrice, CurrencyCode Currency)>();

            // Act & Assert
            Assert.Throws<InvalidOrderException>(() => order.Confirm(productPrices, DateTime.UtcNow));
        }

        [Fact]
        public void Confirm_ShouldThrow_WhenOrderIsAlreadyConfirmed()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var productId = Guid.NewGuid();
            order.AddItem(productId, 1);

            var productPrices = new Dictionary<Guid, (decimal UnitPrice, CurrencyCode Currency)>
            {
                [productId] = (10, CurrencyCode.USD)
            };

            order.Confirm(productPrices, DateTime.UtcNow);

            // Act & Assert
            Assert.Throws<InvalidOrderException>(() => order.Confirm(productPrices, DateTime.UtcNow));
        }

        [Fact]
        public void Confirm_ShouldThrow_WhenOrderIsCompleted()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var productId = Guid.NewGuid();
            order.AddItem(productId, 1);

            var productPrices = new Dictionary<Guid, (decimal UnitPrice, CurrencyCode Currency)>
            {
                [productId] = (10, CurrencyCode.USD)
            };

            order.Confirm(productPrices, DateTime.UtcNow);
            order.Complete();

            // Act & Assert
            Assert.Throws<InvalidOrderException>(() => order.Confirm(productPrices, DateTime.UtcNow));
        }

        [Fact]
        public void Confirm_ShouldThrow_WhenOrderIsCancelled()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.Cancel();

            var productPrices = new Dictionary<Guid, (decimal UnitPrice, CurrencyCode Currency)>();

            // Act & Assert
            Assert.Throws<InvalidOrderException>(() => order.Confirm(productPrices, DateTime.UtcNow));
        }

        [Fact]
        public void Complete_ShouldChangeStatusToCompleted_WhenOrderIsConfirmed()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var productId = Guid.NewGuid();
            order.AddItem(productId, 1);

            var productPrices = new Dictionary<Guid, (decimal UnitPrice, CurrencyCode Currency)>
            {
                [productId] = (10, CurrencyCode.USD)
            };

            order.Confirm(productPrices, DateTime.UtcNow);

            // Act
            order.Complete();

            // Assert
            Assert.Equal(OrderStatus.Completed, order.Status);
        }

        [Fact]
        public void Complete_ShouldThrow_WhenOrderIsPending()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());

            // Act & Assert
            Assert.Throws<InvalidOrderException>(() => order.Complete());
        }

        [Fact]
        public void Complete_ShouldThrow_WhenOrderIsCancelled()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.Cancel();

            // Act & Assert
            Assert.Throws<InvalidOrderException>(() => order.Complete());
        }

        [Fact]
        public void Complete_ShouldThrow_WhenOrderIsAlreadyCompleted()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var productId = Guid.NewGuid();
            order.AddItem(productId, 1);

            var productPrices = new Dictionary<Guid, (decimal UnitPrice, CurrencyCode Currency)>
            {
                [productId] = (10, CurrencyCode.USD)
            };

            order.Confirm(productPrices, DateTime.UtcNow);
            order.Complete();

            // Act & Assert
            Assert.Throws<InvalidOrderException>(() => order.Complete());
        }

        [Fact]
        public void Cancel_ShouldChangeStatusToCancelled_WhenOrderIsPending()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());

            // Act
            order.Cancel();

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order.Status);
        }

        [Fact]
        public void Cancel_ShouldChangeStatusToCancelled_WhenOrderIsConfirmed()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var productId = Guid.NewGuid();
            order.AddItem(productId, 1);

            var productPrices = new Dictionary<Guid, (decimal UnitPrice, CurrencyCode Currency)>
            {
                [productId] = (10, CurrencyCode.USD)
            };

            order.Confirm(productPrices, DateTime.UtcNow);

            // Act
            order.Cancel();

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order.Status);
        }

        [Fact]
        public void Cancel_ShouldThrow_WhenOrderIsAlreadyCompleted()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var productId = Guid.NewGuid();
            order.AddItem(productId, 1);

            var productPrices = new Dictionary<Guid, (decimal UnitPrice, CurrencyCode Currency)>
            {
                [productId] = (10, CurrencyCode.USD)
            };

            order.Confirm(productPrices, DateTime.UtcNow);
            order.Complete();

            // Act & Assert
            Assert.Throws<InvalidOrderException>(() => order.Cancel());
        }

        [Fact]
        public void Cancel_ShouldThrow_WhenOrderIsAlreadyCancelled()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.Cancel();

            // Act & Assert
            Assert.Throws<InvalidOrderException>(() => order.Cancel());
        }

        [Fact]
        public void RemoveItem_ShouldDecreaseQuantity_WhenQuantityIsLessThanCurrentQuantity()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var productId = Guid.NewGuid();

            order.AddItem(productId, 5);

            // Act
            order.DecreaseItem(productId, 2);

            // Assert
            var item = order.Items.First();

            Assert.Equal(3, item.Quantity);
        }

        [Fact]
        public void RemoveItem_ShouldRemoveItem_WhenQuantityReachesZero()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var productId = Guid.NewGuid();

            order.AddItem(productId, 2);

            // Act
            order.DecreaseItem(productId, 2);

            // Assert
            Assert.Empty(order.Items);
        }

        [Fact]
        public void RemoveItem_ShouldThrow_WhenProductDoesNotExist()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());

            // Act & Assert
            Assert.Throws<InvalidOrderItemException>(() => order.DecreaseItem(Guid.NewGuid(), 1));
        }

        [Fact]
        public void RemoveItem_ShouldThrow_WhenQuantityIsGreaterThanCurrentQuantity()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var productId = Guid.NewGuid();

            order.AddItem(productId, 2);

            // Act & Assert
            Assert.Throws<InvalidOrderItemException>(() => order.DecreaseItem(productId, 3));
        }

        [Fact]
        public void RemoveItem_ShouldThrow_WhenOrderIsNotPending()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var productId = Guid.NewGuid();

            order.AddItem(productId, 2);

            var prices = new Dictionary<Guid, (decimal UnitPrice, CurrencyCode Currency)>
            {
                [productId] = (10, CurrencyCode.USD)
            };

            order.Confirm(prices, DateTime.UtcNow);

            // Act & Assert
            Assert.Throws<InvalidOrderException>(() => order.DecreaseItem(productId, 1));
        }
    }
}
