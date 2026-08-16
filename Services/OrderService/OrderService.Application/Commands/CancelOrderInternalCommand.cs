using OrderService.Application.Abstractions;

namespace OrderService.Application.Commands
{
    public class CancelOrderInternalCommand : ICommand
    {
        public Guid OrderId { get; }

        public CancelOrderInternalCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
