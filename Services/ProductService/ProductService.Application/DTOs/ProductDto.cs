using ProductService.Domain.Value_Objects;


namespace ProductService.Application.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public Money Price { get; set; } = null!;
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public Guid CategoryId { get; set; }
    }
}
