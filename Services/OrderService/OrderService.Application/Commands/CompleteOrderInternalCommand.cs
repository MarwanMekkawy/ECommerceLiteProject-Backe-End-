using OrderService.Application.Abstractions;

namespace OrderService.Application.Commands
{
    public class CompleteOrderInternalCommand : ICommand
    {
        public Guid OrderId { get; }

        public CompleteOrderInternalCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
