namespace Gateway.API.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public CurrencyCode Currency { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public Guid CategoryId { get; set; }
    }
}
