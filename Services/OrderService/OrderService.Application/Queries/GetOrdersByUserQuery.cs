using OrderService.Application.Abstractions;
using OrderService.Domain.Orders;

namespace OrderService.Application.Queries
{
    public class GetOrdersByUserQuery : IQuery<IReadOnlyList<Order>>
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
