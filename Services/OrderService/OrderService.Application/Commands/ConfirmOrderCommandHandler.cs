using Domain.Exceptions;
using OrderService.Application.Abstractions;
using OrderService.Domain.Contracts;

namespace OrderService.Application.Commands
{
    public class ConfirmOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork uow) : ICommandHandler<ConfirmOrderCommand>
    {
        public async Task HandleAsync(ConfirmOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdTrackedAsync(command.OrderId, cancellationToken);

            if (order is null)
                throw new NotFoundException($"Order with Id {command.OrderId} Was NOT FOUND.");

            order.Confirm();

            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
