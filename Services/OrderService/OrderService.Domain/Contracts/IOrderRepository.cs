using OrderService.Domain.Orders;

namespace OrderService.Domain.Contracts 
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order, CancellationToken cancellationToken = default);
        Task<Order?> GetByIdUnTrackedAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task<Order?> GetByIdTrackedAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task<Order?> GetByIdAndUserIdUnTrackedAsync(Guid orderId, Guid userId, CancellationToken cancellationToken = default);
        Task<Order?> GetByIdAndUserIdTrackedAsync(Guid orderId, Guid userId, CancellationToken cancellationToken = default);
        Task<Order?> GetLatestByUserIdUnTrackedAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Order?> GetPendingByUserIdTrackedAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Order>> GetPagedByUserIdAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Order>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
