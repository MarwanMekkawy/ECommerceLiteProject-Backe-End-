using OrderService.Domain.Enums;
using OrderService.Domain.Exceptions.DomainExceptions;



namespace OrderService.Domain.Orders
{
    public class OrderItem
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public Guid OrderId { get; private set; }

        // Price snapshot when the order is confirmed
        public decimal UnitPrice { get; private set; }
        public CurrencyCode Currency { get; private set; }
        public decimal Total { get; private set; }

        public Order Order { get; private set; } = null!;

        private OrderItem() { }

        public OrderItem(Guid orderId, Guid productId, int quantity)
        {
            if (orderId == Guid.Empty)
                throw new InvalidOrderItemException("OrderId cannot be empty.");

            if (productId == Guid.Empty)
                throw new InvalidOrderItemException("ProductId cannot be empty.");

            if (quantity <= 0)
                throw new InvalidOrderItemException("Quantity must be greater than zero.");

            Id = Guid.NewGuid();
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
        }

        internal void IncreaseQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new InvalidOrderItemException("Quantity must be greater than zero.");

            Quantity += quantity;
        }
        internal void DecreaseQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new InvalidOrderItemException("Quantity must be greater than zero.");

            if (quantity > Quantity)
                throw new InvalidOrderItemException("Cannot remove more than the current quantity.");

            Quantity -= quantity;
        }

        internal void SetPriceSnapshot(decimal unitPrice, CurrencyCode currency)
        {
            if (unitPrice < 0)
                throw new InvalidOrderItemException("Unit price cannot be negative.");

            UnitPrice = unitPrice;
            Currency = currency;
            Total = unitPrice * Quantity;
        }  
    }
}
