using OrderService.Domain.Orders;

namespace OrderService.Domain.Contracts 
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task AddAsync(Order order, CancellationToken cancellationToken = default);
        Task<Order?> GetByIdTrackedAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Order>> GetPagedByUserIdAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
