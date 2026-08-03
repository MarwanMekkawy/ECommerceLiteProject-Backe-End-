using ProductService.Domain.Enums;


namespace ProductService.Application.DTOs
{
    public class CreateProductRequestDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public CurrencyCode Currency { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
    }
}
