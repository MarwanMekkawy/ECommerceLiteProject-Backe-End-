using Domain.Exceptions;
using OrderService.Application.Abstractions;
using OrderService.Domain.Contracts;

namespace OrderService.Application.Commands
{
    public class IncreaseOrderItemCommandHandler(IOrderRepository orderRepository, IUnitOfWork uow) : ICommandHandler<IncreaseOrderItemCommand>
    {
        public async Task HandleAsync(IncreaseOrderItemCommand command, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdAndUserIdTrackedAsync(command.OrderId, command.UserId, cancellationToken);

            if (order is null)
                throw new NotFoundException($"Order with Id {command.OrderId} Was NOT FOUND.");

            order.IncreaseItem(command.ProductId, command.Quantity);

            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
