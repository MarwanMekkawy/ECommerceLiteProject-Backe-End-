using OrderService.Application.Abstractions;
using OrderService.Domain.Contracts;

namespace OrderService.Application.Commands
{
    public class RemoveOrderItemCommandHandler(IOrderRepository orderRepository, IUnitOfWork uow) : ICommandHandler<RemoveOrderItemCommand>
    {
        public Task HandleAsync(RemoveOrderItemCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
