

using ProductService.Application.Abstractions;
using ProductService.Domain.Enums;
using ProductService.Domain.Value_Objects;

namespace ProductService.Application.Commands.Products
{
    public class UpdateProductCommand : ICommand
    {
        public Guid ProductId { get; }
        public string NewName { get; }
        public string? NewDescription { get; }
        public Money NewPrice { get; } 
        public Guid NewCategoryId { get; }

        public UpdateProductCommand
            (Guid productID, string newName, string? newDescription, decimal newAmount, CurrencyCode newCurrency, decimal discount, Guid newCategoryId)
        {
            ProductId = productID;
            NewName = newName;
            NewDescription = newDescription;
            NewCategoryId = newCategoryId;

            NewPrice = new Money(newAmount,newCurrency).ApplyDiscount(discount);
        }
    }
}
