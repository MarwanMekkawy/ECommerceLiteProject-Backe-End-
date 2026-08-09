using OrderService.Domain.Exceptions.DomainExceptions;



namespace OrderService.Domain.Orders
{
    public class OrderItem
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }

        public OrderItem(Guid productId, int quantity)
        {
            if (productId == Guid.Empty)
                throw new InvalidOrderItemException("ProductId cannot be empty.");

            if (quantity <= 0)
                throw new InvalidOrderItemException("Quantity must be greater than zero.");

            Id = Guid.NewGuid();
            ProductId = productId;
            Quantity = quantity;
        }

        internal void IncreaseQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new InvalidOrderItemException("Quantity must be greater than zero.");

            Quantity += quantity;
        }
    }
}
