using OrderService.Domain.Exceptions.DomainExceptions;
using OrderService.Domain.Orders;
using Xunit;
using Xunit.Runner.Common;

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
        public void CreateOrder_WithEmptyUserId_ShouldThrow()
        {
            var userId = Guid.Empty;

            var action = () => new Order(userId);

            Assert.Throws<InvalidOrderException>(action);
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
        public void AddItem_ShouldIncreaseQuantity_WhenProductAlreadyExists()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var order = new Order(userId);
            order.AddItem(productId, 2);
            order.AddItem(productId, 2);

            Assert.Equal(1, order.Items.Count(x => x.ProductId == productId));
            Assert.Equal(4, order.Items.Single().Quantity);
        }        
        [Fact]
        public void Confirm_ShouldChangeStatusToConfirmed_WhenOrderIsPending()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());

            // Act
            order.Confirm();

            // Assert
            Assert.Equal(OrderStatus.Confirmed, order.Status);
        }

        [Fact]
        public void Confirm_ShouldThrow_WhenOrderIsAlreadyConfirmed()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.Confirm();

            // Act & Assert
            Assert.Throws<InvalidOrderException>(() => order.Confirm());
        }

        [Fact]
        public void Confirm_ShouldThrow_WhenOrderIsCompleted()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.Confirm();
            order.Complete();

            // Act & Assert
            Assert.Throws<InvalidOrderException>(() => order.Confirm());
        }

        [Fact]
        public void Confirm_ShouldThrow_WhenOrderIsCancelled()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.Cancel();

            // Act & Assert
            Assert.Throws<InvalidOrderException>(() => order.Confirm());
        }


        [Fact]
        public void Complete_ShouldChangeStatusToCompleted_WhenOrderIsConfirmed()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.Confirm();

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
            order.Confirm();
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
            order.Confirm();

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
            order.Confirm();
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
    }
}
