using OrderService.Application.Abstractions;

namespace OrderService.Application.Commands
{
    public class ConfirmOrderCommand : ICommand
    {
        public Guid OrderId { get; }

        public ConfirmOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
