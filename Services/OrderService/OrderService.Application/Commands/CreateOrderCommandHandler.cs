using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;

namespace OrderService.Application.Commands
{
    public class CreateOrderCommandHandler(IOrderRepository orderRepository, IProductServiceClient productServiceClient, IUnitOfWork uow)
    {
        public async Task Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var order = new Order(command.UserId);
            var decreasedItems = new List<CreateOrderItemDto>();

            try
            {
                foreach (var item in command.Items)
                {
                    await productServiceClient.DecreaseStockAsync(item.ProductId,item.Quantity, cancellationToken);

                    decreasedItems.Add(item);

                    order.AddItem(item.ProductId, item.Quantity);
                }
            }
            catch
            {
                foreach (var item in decreasedItems)
                {
                    await productServiceClient.IncreaseStockAsync(item.ProductId, item.Quantity, cancellationToken);
                }
                throw;
            }

            await orderRepository.AddAsync(order, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
