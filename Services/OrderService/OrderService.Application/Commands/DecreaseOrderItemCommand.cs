using OrderService.Application.Abstractions;

namespace OrderService.Application.Commands
{
    public class DecreaseOrderItemCommand : ICommand
    {
        public Guid UserId { get; }
        public Guid OrderId { get; }
        public Guid ProductId { get; }
        public int Quantity { get; }

        public DecreaseOrderItemCommand(Guid userId, Guid orderId, Guid productId, int quantity)
        {
            UserId = userId;
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
