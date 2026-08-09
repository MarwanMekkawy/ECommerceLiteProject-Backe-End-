using OrderService.Domain.Orders;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid orderId);
    Task AddAsync(Order order);
}