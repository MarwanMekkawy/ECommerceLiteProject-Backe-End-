namespace Gateway.API.DTOs
{
    public class CheckoutOrderDto
    {
        public Guid OrderId { get; set; }
        public IReadOnlyList<CheckoutOrderItemDto> Items { get; set; } = [];
        public decimal Total { get; set; }
        public string Currency { get; set; } = string.Empty;

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
        public string Status { get; set; } = string.Empty;
        public bool IsCancelledDueToExpiry { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public decimal Total { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = [];
    }
    public class OrderItemResponseDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Currency { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}
