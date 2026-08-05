using ProductService.Domain.Enums;


namespace ProductService.Application.DTOs
{
    public class UpdateProductRequestDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public CurrencyCode Currency { get; set; }
        public Guid CategoryId { get; set; }
    }
}
