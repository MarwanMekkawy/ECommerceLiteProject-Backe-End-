using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;

namespace OrderService.Application.Queries
{
    public class GetOrderByIdQuery : IQuery<OrderResponseDto?>
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
