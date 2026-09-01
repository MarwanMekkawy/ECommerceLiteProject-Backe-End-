using OrderService.Domain.Enums;
using OrderService.Domain.Orders;

namespace OrderService.Application.DTOs
{
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
}
