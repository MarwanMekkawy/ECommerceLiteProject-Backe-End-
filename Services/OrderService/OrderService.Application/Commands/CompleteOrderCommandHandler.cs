using Domain.Exceptions;
using OrderService.Application.Abstractions;
using OrderService.Domain.Contracts;

namespace OrderService.Application.Commands
{
    public class CompleteOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork uow) : ICommandHandler<CompleteOrderCommand>
    {
        public async Task HandleAsync(CompleteOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdTrackedAsync(command.OrderId, cancellationToken);

            if (order is null)
                throw new NotFoundException($"Order with Id {command.OrderId} Was NOT FOUND.");

            order.Complete();
            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
