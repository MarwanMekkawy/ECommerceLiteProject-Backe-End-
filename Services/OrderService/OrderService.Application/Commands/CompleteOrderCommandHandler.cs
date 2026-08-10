using OrderService.Application.Abstractions;

namespace OrderService.Application.Commands
{
    public class CompleteOrderCommandHandler : ICommandHandler<CompleteOrderCommand>
    {
        public Task HandleAsync(CompleteOrderCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
