using OrderService.Application.Abstractions;
using OrderService.Domain.Orders;

namespace OrderService.Application.Queries
{
    public class GetLatestOrderQuery : IQuery<Order?>
    {
        public Guid UserId {  get; }
        public GetLatestOrderQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
