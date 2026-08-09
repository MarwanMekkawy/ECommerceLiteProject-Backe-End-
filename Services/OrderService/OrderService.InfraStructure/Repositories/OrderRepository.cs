using OrderService.Domain.Orders;

namespace OrderService.InfraStructure.Repositories
{
    public class OrderRepository(OrderDbContext _context) : IOrderRepository
    {
        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public Task<Order?> GetByIdAsync(Guid orderId)
        {
            throw new NotImplementedException();
        }
    }
}
