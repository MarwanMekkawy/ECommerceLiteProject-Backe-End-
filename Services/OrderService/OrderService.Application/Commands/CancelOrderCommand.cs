using OrderService.Application.Abstractions;

namespace OrderService.Application.Commands
{
    public class CancelOrderCommand : ICommand
    {
        public Guid OrderId { get; }
        public Guid UserId { get; }

        public CancelOrderCommand(Guid userId, Guid orderId)
        {
            OrderId = orderId;
            UserId = userId;
        }
    }
}
