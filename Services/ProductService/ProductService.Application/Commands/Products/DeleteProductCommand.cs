using ProductService.Application.Abstractions;


namespace ProductService.Application.Commands.Products
{
    public class DeleteProductCommand : ICommand
    {
        public Guid ProductId { get; }

        public DeleteProductCommand(Guid productId)
        {
            ProductId = productId;
        }
    }
}
