using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;

namespace OrderService.Application.Queries
{
    public class GetOrderByIdAdminQuery : IQuery<OrderResponseDto?>
    {
        public Guid OrderId { get; }

        public GetOrderByIdAdminQuery(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
