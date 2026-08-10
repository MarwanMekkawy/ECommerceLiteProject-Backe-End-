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

        public async Task<Order?> GetByIdUntrackedAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders.AsNoTracking().Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);
        }

        public async Task<Order?> GetByIdTrackedAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders.AsNoTracking().Include(x => x.Items).Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetPagedByUserIdAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Orders.AsNoTracking().Include(x => x.Items).Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        }
    }
}
