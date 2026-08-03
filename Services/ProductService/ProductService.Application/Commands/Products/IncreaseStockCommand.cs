using ProductService.Application.Abstractions;


namespace ProductService.Application.Commands.Products
{
    public class IncreaseStockCommand : ICommand<int>
    {
        public Guid ProductId { get; }
        public int Quantity { get; }

        public IncreaseStockCommand(Guid productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
