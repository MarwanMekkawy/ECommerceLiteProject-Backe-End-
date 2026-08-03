using ProductService.Domain.Value_Objects;


namespace ProductService.Domain.Entities
{
    public class Product : BaseEntity
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public Money Price { get; private set; } = null!;
        public int StockQuantity { get; private set; }
        public bool IsActive { get; private set; }

        public Guid CategoryId { get; private set; }
        public Category Category { get; private set; } = null!;

        private Product() { }

        public Product(string name, string? description, Money price, int stockQuantity, Guid categoryId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name is required.");

            if (stockQuantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative.");

            Name = name.Trim();
            Description = description?.Trim();
            Price = price;
            StockQuantity = stockQuantity;
            CategoryId = categoryId;
            IsActive = true;
        }

        public void ChangePrice(Money price)
        {
            Price = price;
        }

        public void IncreaseStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            StockQuantity += quantity;
        }

        public void DecreaseStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            if (quantity > StockQuantity)
                throw new InvalidOperationException("Insufficient stock.");

            StockQuantity -= quantity;
        }

        public void Activate()
        {
            if (IsActive)
                throw new InvalidOperationException("Product is already active.");

            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new InvalidOperationException("Product is already inactive.");

            IsActive = false;
        }

        public void Update(string name, string? description, Money price, Guid categoryId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name is required.");

            Name = name.Trim();
            Description = description?.Trim();
            Price = price;
            CategoryId = categoryId;
        }
    }
}
