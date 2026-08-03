using ProductService.Application.Abstractions;


namespace ProductService.Application.Commands.Products
{
    public class DecreaseStockCommand : ICommand<int>
    {
        public Guid ProductId { get; }
        public int Quantity { get; }

        public DecreaseStockCommand(Guid productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
