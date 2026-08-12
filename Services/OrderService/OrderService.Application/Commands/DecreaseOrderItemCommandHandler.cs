using Domain.Exceptions;
using OrderService.Application.Abstractions;
using OrderService.Domain.Contracts;

namespace OrderService.Application.Commands
{
    public class DecreaseOrderItemCommandHandler(IOrderRepository orderRepository, IUnitOfWork uow) : ICommandHandler<DecreaseOrderItemCommand>
    {
        public async Task HandleAsync(DecreaseOrderItemCommand command, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdAndUserIdTrackedAsync(command.OrderId, command.UserId, cancellationToken);

            if (order is null)
                throw new NotFoundException($"Order with Id {command.OrderId} was NOT FOUND.");

            order.DecreaseItem(command.ProductId, command.Quantity);

            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
