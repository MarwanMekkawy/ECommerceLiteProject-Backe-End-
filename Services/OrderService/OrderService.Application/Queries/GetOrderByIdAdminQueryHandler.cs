using OrderService.Application.Abstractions;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;

namespace OrderService.Application.Queries
{
    public class GetOrderByIdAdminQueryHandler(IOrderRepository orderRepository) : IQueryHandler<GetOrderByIdAdminQuery, Order?>
    {
        public Task<Order?> HandleAsync(GetOrderByIdAdminQuery query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
