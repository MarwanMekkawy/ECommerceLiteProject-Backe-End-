using OrderService.Application.Abstractions;
using OrderService.Domain.Orders;

namespace OrderService.Application.Queries
{
    public class GetAllOrdersQuery : IQuery<IReadOnlyList<Order>>
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
