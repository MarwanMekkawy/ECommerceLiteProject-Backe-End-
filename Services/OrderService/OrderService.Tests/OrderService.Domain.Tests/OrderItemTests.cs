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
    }
}
