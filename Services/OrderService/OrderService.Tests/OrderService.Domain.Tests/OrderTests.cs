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
    }
}
