using OrderService.Domain.Enums;

namespace OrderService.Application.DTOs
{
    public class CheckoutOrderDto
    {
        public Guid OrderId { get; set; }
        public IReadOnlyList<CheckoutOrderItemDto> Items { get; set; } = [];
        public decimal Total { get; set; }
        public CurrencyCode Currency { get; set; }
    }
}
