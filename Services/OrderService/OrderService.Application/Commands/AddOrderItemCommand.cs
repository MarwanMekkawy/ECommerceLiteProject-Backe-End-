using OrderService.Application.Abstractions;

namespace OrderService.Application.Commands
{
    public class AddOrderItemCommand : ICommand
    {
        public Guid UserId { get; }
        public Guid ProductId { get; }
        public int Quantity { get; }

        public AddOrderItemCommand(Guid userId, Guid productId, int quantity)
        {
            UserId = userId;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
