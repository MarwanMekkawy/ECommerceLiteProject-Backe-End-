using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;

namespace OrderService.Application.Commands
{
    public class CreateOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork uow) : ICommandHandler<CreateOrderCommand>
    {
        public async Task HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var order = new Order(command.UserId);

            foreach (var item in command.Items)
            {
                order.AddItem(item.ProductId, item.Quantity);
            }

            await orderRepository.AddAsync(order, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
