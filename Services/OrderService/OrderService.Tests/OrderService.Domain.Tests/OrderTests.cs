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
    }
}
