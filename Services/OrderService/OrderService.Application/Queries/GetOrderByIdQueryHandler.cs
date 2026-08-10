using OrderService.Application.Abstractions;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;

namespace OrderService.Application.Queries
{
    public class GetOrderByIdQueryHandler(IOrderRepository orderRepository) : IQueryHandler<GetOrderByIdQuery, Order?>
    {
        public async Task<Order?> HandleAsync(GetOrderByIdQuery query, CancellationToken cancellationToken = default)
        {
            return await orderRepository.GetByIdUntrackedAsync(query.OrderId, cancellationToken);
        }
    }
}
