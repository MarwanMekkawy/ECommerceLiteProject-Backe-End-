using ProductService.Domain.Enums;


namespace ProductService.Application.DTOs
{
    public class ProductWithCategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public CurrencyCode Currency { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public bool CategoryIsActive { get; set; }
    }
}
