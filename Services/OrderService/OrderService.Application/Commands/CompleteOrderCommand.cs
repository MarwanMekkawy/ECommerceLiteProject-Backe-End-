using OrderService.Application.Abstractions;

namespace OrderService.Application.Commands
{
    public class CompleteOrderCommand : ICommand
    {
        public Guid OrderId { get; }

        public CompleteOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
