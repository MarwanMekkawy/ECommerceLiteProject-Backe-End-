using OrderService.Application.Abstractions;

namespace OrderService.Application.Commands
{
    public class IncreaseOrderItemCommand : ICommand
    {
        public Guid UserId { get; }
        public Guid OrderId { get; }
        public Guid ProductId { get; }
        public int Quantity { get; }

        public IncreaseOrderItemCommand(Guid userId, Guid orderId, Guid productId, int quantity)
        {
            UserId = userId;
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
