using ProductService.Application.Abstractions;


namespace ProductService.Application.Commands.Products
{
    public class ActivateProductCommand : ICommand
    {
        public Guid ProductId { get; }

        public ActivateProductCommand(Guid productID)
        {
            ProductId = productID;
        }
    }
}
