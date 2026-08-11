using OrderService.Application.Abstractions;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;

namespace OrderService.Application.Queries
{
    public class GetOrderByIdQueryHandler(IOrderRepository orderRepository) : IQueryHandler<GetOrderByIdQuery, Order?>
    {
        public async Task<Order?> HandleAsync(GetOrderByIdQuery query, CancellationToken cancellationToken)
        {
            return await orderRepository.GetByIdAndUserIdUnTrackedAsync(query.UserId, query.OrderId, cancellationToken);
        }
    }
}
