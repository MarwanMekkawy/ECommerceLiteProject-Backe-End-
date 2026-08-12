using OrderService.Application.Abstractions;
using OrderService.Domain.Contracts;

namespace OrderService.Application.Commands
{
    public class AddOrderItemCommandHandler(IOrderRepository orderRepository, IUnitOfWork uow) : ICommandHandler<AddOrderItemCommand>
    {
        public Task HandleAsync(AddOrderItemCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
