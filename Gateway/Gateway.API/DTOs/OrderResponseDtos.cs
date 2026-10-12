namespace Gateway.API.DTOs
{
    public class CheckoutOrderDto
    {
        public Guid OrderId { get; set; }
        public IReadOnlyList<CheckoutOrderItemDto> Items { get; set; } = [];
        public decimal Total { get; set; }
        public CurrencyCode Currency { get; set; }

        public Guid PaymentId { get; set; }
        public string ClientSecret { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
    }
    public class CheckoutOrderItemDto
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }
    public class OrderResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public OrderStatus Status { get; set; }
        public bool IsCancelledDueToExpiry { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public decimal Total { get; set; }
        public CurrencyCode Currency { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = [];
    }
    public class OrderItemResponseDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public CurrencyCode Currency { get; set; }
        public decimal Total { get; set; }
    }
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Completed,
        Cancelled,
        CancelledAndRefunded
    }
    public enum CurrencyCode
    {
        USD,
        EUR,
        EGP
    }
}
