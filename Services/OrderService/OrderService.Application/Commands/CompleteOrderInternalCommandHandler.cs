using Domain.Exceptions;
using OrderService.Application.Abstractions;
using OrderService.Application.Abstractions.ClientsAbstractions;
using OrderService.Domain.Contracts;

namespace OrderService.Application.Commands
{
    public class CompleteOrderInternalCommandHandler(IOrderRepository orderRepository, INotificationServiceClient notificationServiceClient, IUnitOfWork uow) : ICommandHandler<CompleteOrderInternalCommand>
    {
        public async Task HandleAsync(CompleteOrderInternalCommand command, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdTrackedAsync(command.OrderId, cancellationToken);

            if (order is null)
                throw new NotFoundException($"Order with Id {command.OrderId} Was NOT FOUND.");

            if (order.IsCancelledDueToExpiry) 
                throw new ConflictException($"Order with Id [{command.OrderId}] expired at {order.ExpiresAt:O} and can no longer be Completed. Please create a new order.");

            order.Complete();
            await uow.SaveChangesAsync(cancellationToken);

            var items = order.Items.Select
                (item => $"<strong>{item.ProductName}:</strong> {item.Quantity} X {item.UnitPrice:N2} = {item.Total:N2}{item.Currency}").ToList();
            await notificationServiceClient.SendOrderCompletedAsync
                (order.UserId, order.CustomerEmail, "Customer", order.Id, items, order.Total, order.Currency.ToString(), cancellationToken);
        }
    }
}
