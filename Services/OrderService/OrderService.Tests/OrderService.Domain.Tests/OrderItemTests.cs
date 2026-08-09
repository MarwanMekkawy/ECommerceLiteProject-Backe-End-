using OrderService.Domain.Exceptions.DomainExceptions;
using OrderService.Domain.Orders;
using Xunit;

namespace OrderService.Domain.Tests
{
    public class OrderItemTests
    {
        [Fact]
        public void Constructor_ShouldThrow_WhenProductIdIsEmpty()
        {
            var productId = Guid.Empty;
            var quantity = 1;

            var action = () => new OrderItem(productId, quantity);

            Assert.Throws<InvalidOrderItemException>(action);
        }
        [Fact]
        public void Constructor_ShouldThrow_WhenQuantityIsZero()
        {
            var productId = Guid.NewGuid();
            var quantity = 0;

            var action = () => new OrderItem(productId, quantity);

            Assert.Throws<InvalidOrderItemException>(action);
        }
        [Fact]
        public void Constructor_ShouldThrow_WhenQuantityIsNegative()
        {
            var productId = Guid.NewGuid();
            var quantity = -1;

            var action = () => new OrderItem(productId, quantity);

            Assert.Throws<InvalidOrderItemException>(action);
        }
        [Fact]
        public void Constructor_ShouldCreateItem_WhenArgumentsAreValid()
        {
            var productId = Guid.NewGuid();
            var quantity = 2;

            var item = new OrderItem(productId, quantity);

            Assert.NotEqual(Guid.Empty, item.Id);
            Assert.Equal(productId, item.ProductId);
            Assert.Equal(quantity, item.Quantity);
        }
    }
}
