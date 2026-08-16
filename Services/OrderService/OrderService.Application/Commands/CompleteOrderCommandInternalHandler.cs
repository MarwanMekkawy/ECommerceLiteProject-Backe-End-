using Domain.Exceptions;
using OrderService.Application.Abstractions;
using OrderService.Domain.Contracts;

namespace OrderService.Application.Commands
{
    public class CompleteOrderCommandInternalHandler(IOrderRepository orderRepository, IUnitOfWork uow) : ICommandHandler<CompleteOrderInternalCommand>
    {
        public async Task HandleAsync(CompleteOrderInternalCommand command, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdTrackedAsync(command.OrderId, cancellationToken);

            if (order is null)
                throw new NotFoundException($"Order with Id {command.OrderId} Was NOT FOUND.");

            order.Complete();
            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
