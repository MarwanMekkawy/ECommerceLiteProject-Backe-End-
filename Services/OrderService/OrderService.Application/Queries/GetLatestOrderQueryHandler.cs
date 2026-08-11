using OrderService.Application.Abstractions;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;


namespace OrderService.Application.Queries
{
    public class GetLatestOrderQueryHandler(IOrderRepository orderRepository) : IQueryHandler<GetLatestOrderQuery, Order?>
    {
        public async Task<Order?> HandleAsync(GetLatestOrderQuery query, CancellationToken cancellationToken)
        {
            return await orderRepository.GetLatestByUserIdUntrackedAsync(query.UserId, cancellationToken);
        }
    }
}
