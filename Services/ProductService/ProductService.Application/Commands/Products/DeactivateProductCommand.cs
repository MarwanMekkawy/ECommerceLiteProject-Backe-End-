using ProductService.Application.Abstractions;


namespace ProductService.Application.Commands.Products
{
    public class DeactivateProductCommand : ICommand
    {
        public Guid ProductId { get; }

        public DeactivateProductCommand(Guid productID)
        {
            ProductId = productID;
        }
    }
}
