using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;

namespace OrderService.Application.Commands
{
    public class CheckoutOrderCommand : ICommand<CheckoutOrderDto>
    {
        public Guid OrderId { get; }

        public CheckoutOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
