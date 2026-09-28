using OrderService.Application.Abstractions;
using OrderService.Application.Abstractions.ClientsAbstractions;
using OrderService.Domain.Contracts;

namespace OrderService.Application.Services
{
    public class CancelExpiredOrdersService
        (IOrderRepository orderRepository, INotificationServiceClient notificationServiceClient, IUnitOfWork uow) 
        : ICancelExpiredOrdersService
    {
        public async Task CancelExpiredAsync(CancellationToken cancellationToken)
        {
            var expiredOrders = await orderRepository.GetConfirmedOrdersPastExpiryDateAsync(cancellationToken);           

            foreach (var order in expiredOrders) 
            {
                order.Expire();
            }

            await uow.SaveChangesAsync(cancellationToken);

            foreach (var order in expiredOrders)
            {
                if(order.IsCancelledDueToExpiry)
                    await notificationServiceClient.SendOrderCancelledDueExpirationAsync(order.UserId, order.CustomerEmail, "Customer", order.Id, cancellationToken);
            }
        }
    }
}