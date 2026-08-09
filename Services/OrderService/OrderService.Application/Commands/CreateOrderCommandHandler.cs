using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;
using OrderService.Domain.Orders;

namespace OrderService.Application.Commands
{
    public class CreateOrderCommandHandler(IOrderRepository orderRepository, IProductServiceClient productServiceClient, IUnitOfWork uow)
    {
        public async Task Handle(CreateOrderCommand command)
        {
            var order = new Order(command.UserId);
            var decreasedItems = new List<CreateOrderItemDto>();

            try
            {
                foreach (var item in command.Items)
                {
                    await productServiceClient.DecreaseStockAsync(item.ProductId,item.Quantity);

                    decreasedItems.Add(item);

                    order.AddItem(item.ProductId, item.Quantity);
                }
            }
            catch
            {
                foreach (var item in decreasedItems)
                {
                    await productServiceClient.IncreaseStockAsync(item.ProductId, item.Quantity);
                }
                throw;
            }

            await orderRepository.AddAsync(order);
            await uow.SaveChangesAsync();
        }
    }
}
