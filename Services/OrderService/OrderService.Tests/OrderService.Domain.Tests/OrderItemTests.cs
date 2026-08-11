using OrderService.Domain.Enums;
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
            var orderId = Guid.NewGuid();
            var productId = Guid.Empty;
            var quantity = 1;

            var action = () => new OrderItem(orderId, productId, quantity);

            Assert.Throws<InvalidOrderItemException>(action);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenQuantityIsZero()
        {
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var quantity = 0;

            var action = () => new OrderItem(orderId, productId, quantity);

            Assert.Throws<InvalidOrderItemException>(action);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenQuantityIsNegative()
        {
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var quantity = -1;

            var action = () => new OrderItem(orderId, productId, quantity);

            Assert.Throws<InvalidOrderItemException>(action);
        }

        [Fact]
        public void Constructor_ShouldCreateItem_WhenArgumentsAreValid()
        {
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var quantity = 2;

            var item = new OrderItem(orderId, productId, quantity);

            Assert.NotEqual(Guid.Empty, item.Id);
            Assert.Equal(productId, item.ProductId);
            Assert.Equal(quantity, item.Quantity);
        }
    }
}
