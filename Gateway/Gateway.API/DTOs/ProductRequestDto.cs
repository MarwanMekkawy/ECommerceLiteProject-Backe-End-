namespace Gateway.API.DTOs
{
    public class CreateCategoryRequestDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
    public class UpdateCategoryRequestDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
    public class CreateProductRequestDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public string Currency { get; set; } = string.Empty;  
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
    }
    public class UpdateProductRequestDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
    }
}
