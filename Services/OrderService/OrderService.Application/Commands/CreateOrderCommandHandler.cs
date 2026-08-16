using OrderService.Application.Abstractions;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;

namespace OrderService.Application.Commands
{
    public class CreateOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork uow) : ICommandHandler<CreateOrderCommand>
    {
        public async Task HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetPendingByUserIdTrackedAsync(command.UserId, cancellationToken);

            if (order is null)
            {
                order = new Order(command.UserId);

                await orderRepository.AddAsync(order, cancellationToken);
            }

            foreach (var item in command.Items)
            {
                order.AddItem(item.ProductId, item.Quantity);
            }

            await uow.SaveChangesAsync(cancellationToken);
        }
    }   
}
