using OrderService.Application.Abstractions;

namespace OrderService.Application.Commands
{
    public class ConfirmOrderCommandHandler : ICommandHandler<ConfirmOrderCommand>
    {
        public Task HandleAsync(ConfirmOrderCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
