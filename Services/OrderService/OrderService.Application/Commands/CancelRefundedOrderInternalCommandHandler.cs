using Domain.Exceptions;
using OrderService.Application.Abstractions;
using OrderService.Application.Abstractions.ClientsAbstractions;
using OrderService.Domain.Contracts;

namespace OrderService.Application.Commands
{
    public class CancelRefundedOrderInternalCommandHandler(IOrderRepository orderRepository, INotificationServiceClient notificationServiceClient, IUnitOfWork uow) : ICommandHandler<CancelRefundedOrderInternalCommand>
    {
        public async Task HandleAsync(CancelRefundedOrderInternalCommand command, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdTrackedAsync(command.OrderId, cancellationToken);

            if (order is null)
                throw new NotFoundException($"Order with Id {command.OrderId} Was NOT FOUND.");

            order.Cancel(true);
            await uow.SaveChangesAsync(cancellationToken);

            var reason = "Order was cancelled due to a refund request.";
            await notificationServiceClient.SendOrderCancelledAsync(order.UserId, order.CustomerEmail, "Customer", order.Id, reason, cancellationToken);
        }
    }
}