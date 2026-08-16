using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;

namespace OrderService.Application.Queries
{
    public class GetOrdersByUserQuery : IQuery<IReadOnlyList<OrderResponseDto>>
    {
        public Guid UserId { get; }
        public int PageNumber { get; }
        public int PageSize { get; }

        public GetOrdersByUserQuery(Guid userId, int pageNumber, int pageSize)
        {
            UserId = userId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
