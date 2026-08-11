using OrderService.Application.Abstractions;
using OrderService.Domain.Orders;

namespace OrderService.Application.Queries
{
    public class GetOrderByIdQuery : IQuery<Order?>
    {
        public Guid OrderId { get; }
        public Guid UserId { get; }

        public GetOrderByIdQuery(Guid userId, Guid orderId)
        {
            OrderId = orderId;
            UserId = userId;
        }
    }
}
