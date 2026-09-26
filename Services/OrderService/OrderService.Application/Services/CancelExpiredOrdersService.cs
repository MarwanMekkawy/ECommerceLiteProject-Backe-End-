using OrderService.Application.Abstractions;
using OrderService.Application.Abstractions.ClientsAbstractions;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;

namespace OrderService.Application.Services
{
    public class CancelExpiredOrdersService
        (IOrderRepository orderRepository, IProductServiceClient productServiceClient, INotificationServiceClient notificationServiceClient, IUnitOfWork uow) 
        : ICancelExpiredOrdersService
    {
        public async Task CancelExpiredAsync(CancellationToken cancellationToken)
        {
            var expiredOrders = await orderRepository.GetConfirmedOrdersPastExpiryDateAsync(cancellationToken);           

            foreach (var order in expiredOrders) 
            {
                foreach (var item in order.Items)
                {
                    await productServiceClient.IncreaseStockAsync(item.ProductId, item.Quantity, cancellationToken);
                }
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
