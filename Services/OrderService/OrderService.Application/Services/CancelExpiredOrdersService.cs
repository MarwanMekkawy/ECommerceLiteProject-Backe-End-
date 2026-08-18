using OrderService.Application.Abstractions;
using OrderService.Domain.Contracts;

namespace OrderService.Application.Services
{
    public class CancelExpiredOrdersService(IOrderRepository orderRepository, IProductServiceClient productServiceClient, IUnitOfWork uow) : ICancelExpiredOrdersService
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
        }
    }
}
