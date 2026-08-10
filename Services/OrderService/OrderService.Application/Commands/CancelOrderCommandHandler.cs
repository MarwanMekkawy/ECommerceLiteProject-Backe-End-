using Domain.Exceptions;
using OrderService.Application.Abstractions;
using OrderService.Domain.Contracts;

namespace OrderService.Application.Commands
{
    public class CancelOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork uow) : ICommandHandler<CancelOrderCommand>
    {
        public async Task HandleAsync(CancelOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdTrackedAsync(command.OrderId, cancellationToken);

            if (order is null)
                throw new NotFoundException($"Order with Id {command.OrderId} Was NOT FOUND.");

            order.Cancel();
            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
