using Domain.Exceptions;
using OrderService.Domain.Exceptions.DomainExceptions;



namespace OrderService.Domain.Orders
{
    public class Order
    {
        private readonly List<OrderItem> _items = new();

        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public OrderStatus Status { get; private set; }

        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        private Order() { }

        public Order(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new InvalidOrderException("User ID is required.");

            Id = Guid.NewGuid();
            UserId = userId;
            Status = OrderStatus.Pending;
        }

        public void AddItem(Guid productId, int quantity)
        {
            var existingItem = _items.FirstOrDefault(x => x.ProductId == productId);

            if (existingItem is not null)
            {
                existingItem.IncreaseQuantity(quantity);
                return;
            }

            _items.Add(new OrderItem(productId, quantity));
        }
    }
}
