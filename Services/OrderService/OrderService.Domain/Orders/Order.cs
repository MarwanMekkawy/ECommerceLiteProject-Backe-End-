using OrderService.Domain.Exceptions.DomainExceptions;



namespace OrderService.Domain.Orders
{
    public class Order
    {
        private readonly List<OrderItem> _items = new();

        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

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
            if (productId == Guid.Empty)
                throw new InvalidOrderItemException("ProductId cannot be empty.");

            if (quantity <= 0)
                throw new InvalidOrderItemException("Quantity must be greater than zero.");

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
