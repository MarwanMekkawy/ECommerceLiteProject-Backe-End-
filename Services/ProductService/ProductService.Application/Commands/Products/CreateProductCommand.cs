using ProductService.Application.Abstractions;
using ProductService.Domain.Enums;
using ProductService.Domain.Value_Objects;


namespace ProductService.Application.Commands.Products
{
    public class CreateProductCommand : ICommand<Guid>
    {
        public string Name { get; }
        public string? Description { get; }
        public Money Price { get; }
        public int StockQuantity { get;  }
        public Guid CategoryId { get; }

        public CreateProductCommand
            (string name, string? description, decimal amount, decimal discount, CurrencyCode currency, Guid categoryId, int stockQuantity) 
        {
            Name = name;
            Description = description;
            CategoryId = categoryId;
            StockQuantity =stockQuantity;

            Price = new Money(amount,currency).ApplyDiscount(discount);
        }
    }
}
