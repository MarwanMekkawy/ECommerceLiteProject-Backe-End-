using OrderService.Domain.Enums;

namespace OrderService.Application.DTOs
{
    public class ProductCheckoutDto
    {
        public Guid ProductId { get; set; }
        public decimal Price { get; set; }
        public CurrencyCode Currency { get; set; }
    }
}
