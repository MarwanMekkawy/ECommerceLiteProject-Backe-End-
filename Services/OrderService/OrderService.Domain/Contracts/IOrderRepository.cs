using OrderService.Domain.Orders;

public interface IOrderRepository
{
    Task AddAsync(Order order);
}