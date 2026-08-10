using OrderService.Application.Abstractions;
using OrderService.Domain.Contracts;

namespace OrderService.Application.Commands
{
    public class CancelOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork uow) : ICommandHandler<CancelOrderCommand>
    {
        public Task HandleAsync(CancelOrderCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
