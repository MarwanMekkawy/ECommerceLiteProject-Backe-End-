using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;

namespace OrderService.Application.Commands
{
    public class CheckoutOrderCommand : ICommand<CheckoutOrderDto>
    {
        public Guid OrderId { get; }
        public Guid UserId { get; }

        public CheckoutOrderCommand(Guid userId, Guid orderId)
        {
            UserId = userId;
            OrderId = orderId;
        }
    }
}
