using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;

namespace OrderService.Application.Queries
{
    public class GetAllOrdersQuery : IQuery<IReadOnlyList<OrderResponseDto>>
    {
        public int PageNumber { get; }
        public int PageSize { get; }

        public GetAllOrdersQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
