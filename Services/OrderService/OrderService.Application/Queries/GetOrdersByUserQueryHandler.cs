using OrderService.Application.Abstractions;
using OrderService.Domain.Orders;

namespace OrderService.Application.Queries
{
    public class GetOrdersByUserQueryHandler : IQueryHandler<GetOrdersByUserQuery, IReadOnlyList<Order>>
    {
        public Task<IReadOnlyList<Order>> HandleAsync(GetOrdersByUserQuery query, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
