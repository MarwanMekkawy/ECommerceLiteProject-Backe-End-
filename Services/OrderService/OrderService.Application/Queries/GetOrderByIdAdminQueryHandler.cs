using OrderService.Application.Abstractions;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;

namespace OrderService.Application.Queries
{
    public class GetOrderByIdAdminQueryHandler(IOrderRepository orderRepository) : IQueryHandler<GetOrderByIdAdminQuery, Order?>
    {
        public async Task<Order?> HandleAsync(GetOrderByIdAdminQuery query, CancellationToken cancellationToken = default)
        {
            return await orderRepository.GetByIdUntrackedAsync(query.OrderId, cancellationToken);
        }
    }
}
