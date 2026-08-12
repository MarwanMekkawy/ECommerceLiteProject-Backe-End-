using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;

namespace OrderService.Application.Queries
{
    public class GetLatestOrderQuery : IQuery<OrderResponseDto?>
    {
        public Guid UserId {  get; }
        public GetLatestOrderQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
