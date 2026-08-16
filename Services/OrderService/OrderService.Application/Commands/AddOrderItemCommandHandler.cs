using OrderService.Application.Abstractions;
using OrderService.Domain.Contracts;
using OrderService.Domain.Orders;

namespace OrderService.Application.Commands
{
    public class AddOrderItemCommandHandler(IOrderRepository orderRepository, IUnitOfWork uow) : ICommandHandler<AddOrderItemCommand>
    {
        public async Task HandleAsync(AddOrderItemCommand command, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetPendingByUserIdTrackedAsync(command.UserId, cancellationToken);

            if (order is null)
            {
                order = new Order(command.UserId);

                await orderRepository.AddAsync(order, cancellationToken);
            }

            order.AddItem(command.ProductId, command.Quantity);

            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
