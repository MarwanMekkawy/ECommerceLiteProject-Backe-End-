using OrderService.Application.Abstractions;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;

namespace OrderService.Application.Queries
{
    public class GetAllOrdersQueryHandler(IOrderRepository orderRepository) : IQueryHandler<GetAllOrdersQuery, IReadOnlyList<Order>>
    {
        public async Task<IReadOnlyList<Order>> HandleAsync(GetAllOrdersQuery query, CancellationToken cancellationToken)
        {
            return await orderRepository.GetPagedAsync(query.PageNumber, query.PageSize, cancellationToken);
        }
    }
}
