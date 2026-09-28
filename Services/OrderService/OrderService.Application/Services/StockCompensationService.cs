using Microsoft.Extensions.Logging;
using OrderService.Application.Abstractions;
using OrderService.Application.Abstractions.ClientsAbstractions;
using OrderService.Domain.Contracts;

namespace OrderService.Application.Services
{
    public class StockCompensationService(IOrderRepository orderRepository, IProductServiceClient productServiceClient, IUnitOfWork uow, ILogger<StockCompensationService> logger) : IStockCompensationService
    {
        public async Task CompensateAsync(CancellationToken cancellationToken)
        {
            var orders = await orderRepository.GetCancelledOrdersPendingStockCompensationAsync(cancellationToken);

            foreach (var order in orders)
            {
                try
                {
                    foreach (var item in order.Items)
                    {
                        await productServiceClient.IncreaseStockAsync(item.ProductId, item.Quantity, cancellationToken);
                    }

                    order.MarkStockCompensated();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to compensate stock for order {OrderId}.",order.Id);
                }
            }
            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
