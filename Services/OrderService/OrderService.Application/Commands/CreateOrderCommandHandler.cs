using OrderService.Application.Abstractions;
using OrderService.Domain.Orders;

namespace OrderService.Application.Commands
{
    public class CreateOrderCommandHandler(IOrderRepository orderRepository, IProductServiceClient productServiceClient, IUnitOfWork uow)
    {
        public async Task Handle(CreateOrderCommand command)
        {
            var order = new Order(command.UserId);

            foreach (var item in command.Items)
            {
                await productServiceClient.DecreaseStockAsync(item.ProductId, item.Quantity);

                order.AddItem(item.ProductId, item.Quantity);
            }

            await orderRepository.AddAsync(order);
            await uow.SaveChangesAsync();
        }
    }
}
