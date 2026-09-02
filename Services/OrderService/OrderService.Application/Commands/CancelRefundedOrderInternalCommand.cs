using OrderService.Application.Abstractions;

namespace OrderService.Application.Commands
{
    public class CancelRefundedOrderInternalCommand : ICommand
    {
        public Guid OrderId { get; }

        public CancelRefundedOrderInternalCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
