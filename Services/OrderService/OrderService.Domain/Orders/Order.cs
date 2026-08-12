using OrderService.Domain.Enums;
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
        public decimal Total { get; private set; }
        public CurrencyCode Currency { get; private set; }
        public DateTime? ConfirmedAt { get; private set; }
        public DateTime? PaymentExpiresAt { get; private set; }

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

        // add item to order
        public void AddItem(Guid productId, int quantity)
        {
            if (productId == Guid.Empty)
                throw new InvalidOrderItemException("ProductId cannot be empty.");

            if (quantity <= 0)
                throw new InvalidOrderItemException("Quantity must be greater than zero.");

            if (Status != OrderStatus.Pending)
                throw new InvalidOrderException("Items can only be added to pending orders.");

            var existingItem = _items.FirstOrDefault(x => x.ProductId == productId);

            if (existingItem is not null)
            {
                existingItem.IncreaseQuantity(quantity);
                return;
            }

            _items.Add(new OrderItem(Id, productId, quantity));
        }
        // remove items from the order
        public void RemoveItem(Guid productId, int quantity)
        {
            if (productId == Guid.Empty)
                throw new InvalidOrderItemException("ProductId cannot be empty.");

            if (quantity <= 0)
                throw new InvalidOrderItemException("Quantity must be greater than zero.");

            if (Status != OrderStatus.Pending)
                throw new InvalidOrderException("Items can only be removed from pending orders.");

            var existingItem = _items.FirstOrDefault(x => x.ProductId == productId);

            if (existingItem is null)
                throw new InvalidOrderItemException("Product is not in the order.");

            existingItem.DecreaseQuantity(quantity);

            if (existingItem.Quantity == 0)
                _items.Remove(existingItem);
        }
        // confirm the order before payment and snapshot its total price
        public void Confirm(IReadOnlyDictionary<Guid, (decimal UnitPrice, CurrencyCode Currency)> productPrices, DateTime confirmedAt)
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOrderException("Only pending orders can be confirmed.");

            if (_items.Count == 0)
                throw new InvalidOrderException("An order must contain at least one item.");

            CurrencyCode? currency = null;
            decimal total = 0;

            // snapshoting each item price in the order
            foreach (var item in _items)
            {
                if (!productPrices.TryGetValue(item.ProductId, out var price))
                    throw new InvalidOrderException($"Price information for product {item.ProductId} is missing.");

                if (currency is null)
                    currency = price.Currency;
                else if (currency != price.Currency)
                    throw new InvalidOrderException("All order items must use the same currency.");

                item.SetPriceSnapshot(price.UnitPrice, price.Currency);

                total += item.Total;
            }

            if (total <= 0) 
                throw new InvalidOrderException("Order total must be greater than zero.");

            Total = total;
            Currency = currency!.Value;
            ConfirmedAt = confirmedAt;
            // confirmed order keeps snapshot for prices at the time of buying for 3 days window to pay
            PaymentExpiresAt = confirmedAt.Add(TimeSpan.FromDays(3)); 

            Status = OrderStatus.Confirmed;
        }

        public void Complete()
        {
            if (Status != OrderStatus.Confirmed)
                throw new InvalidOrderException("Only confirmed orders can be completed.");

            Status = OrderStatus.Completed;
        }

        public void Cancel()
        {
            if (Status != OrderStatus.Pending && Status != OrderStatus.Confirmed)
                throw new InvalidOrderException("Only pending or confirmed orders can be cancelled.");

            Status = OrderStatus.Cancelled;
        }
    }
}
