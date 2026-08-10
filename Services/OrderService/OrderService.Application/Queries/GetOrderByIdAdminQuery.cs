using OrderService.Application.Abstractions;
using OrderService.Domain.Orders;

namespace OrderService.Application.Queries
{
    public class GetOrderByIdAdminQuery : IQuery<Order?>
    {
        public Guid OrderId { get; }

        public GetOrderByIdAdminQuery(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
