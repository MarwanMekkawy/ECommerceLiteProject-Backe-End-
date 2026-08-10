using OrderService.Application.Abstractions;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;

namespace OrderService.Application.Queries
{
    public class GetOrdersByUserQueryHandler(IOrderRepository orderRepository) : IQueryHandler<GetOrdersByUserQuery, IReadOnlyList<Order>>
    {
        public async Task<IReadOnlyList<Order>> HandleAsync(GetOrdersByUserQuery query, CancellationToken cancellationToken = default)
        {
            return await orderRepository.GetPagedByUserIdAsync(query.UserId, query.PageNumber, query.PageSize, cancellationToken);
        }
    }
}
