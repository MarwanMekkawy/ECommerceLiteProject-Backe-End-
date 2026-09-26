using Domain.Exceptions;
using OrderService.Application.Abstractions;
using OrderService.Application.Abstractions.ClientsAbstractions;
using OrderService.Domain.Contracts;

namespace OrderService.Application.Commands
{
    public class CancelOrderCommandHandler(IOrderRepository orderRepository, INotificationServiceClient notificationServiceClient, IUnitOfWork uow) : ICommandHandler<CancelOrderCommand>
    {
        public async Task HandleAsync(CancelOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdAndUserIdTrackedAsync(command.OrderId, command.UserId, cancellationToken);

            if (order is null)
                throw new NotFoundException($"Order with Id {command.OrderId} Was NOT FOUND.");

            order.Cancel();
            await uow.SaveChangesAsync(cancellationToken);

            string reason = "Order Was cancelled by The User.";
            await notificationServiceClient.SendOrderCancelledAsync(order.UserId, order.CustomerEmail, command.UserName, order.Id,reason,cancellationToken);
        }
    }
}
