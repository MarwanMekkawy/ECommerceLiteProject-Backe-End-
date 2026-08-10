using OrderService.Application.Abstractions;

namespace OrderService.Application.Commands
{
    public class CancelOrderCommand : ICommand
    {
        public Guid OrderId { get; }

        public CancelOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
