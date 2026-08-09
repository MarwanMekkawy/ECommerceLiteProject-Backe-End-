using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Orders;

namespace OrderService.InfraStructure.Repositories
{
    public class OrderRepository(OrderDbContext _context) : IOrderRepository
    {
        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task<Order?> GetByIdAsync(Guid orderId)
        {
            return await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        }
    }
}
