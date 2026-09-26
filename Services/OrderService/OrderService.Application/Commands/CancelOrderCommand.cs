using OrderService.Application.Abstractions;

namespace OrderService.Application.Commands
{
    public class CancelOrderCommand : ICommand
    {
        public Guid OrderId { get; }
        public Guid UserId { get; }
        public string UserName { get; }

        public CancelOrderCommand(Guid userId, Guid orderId, string userName)
        {
            OrderId = orderId;
            UserId = userId;
            UserName = userName;
        }
    }
}
