using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Orders;
using OrderService.InfraStructure;

namespace OrderService.Domain.Contracts
{
    public class OrderRepository(OrderDbContext _context) : IOrderRepository
    {
        public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            await _context.Orders.AddAsync(order, cancellationToken);
        }

        public async Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);
        }

        public Task<Order?> GetByIdTrackedAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Order>> GetPagedByUserIdAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
