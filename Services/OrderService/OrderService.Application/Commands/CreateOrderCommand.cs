using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;

namespace OrderService.Application.Commands
{
    public class CreateOrderCommand : ICommand
    {
        public Guid UserId { get; }

        public IReadOnlyCollection<CreateOrderItemDto> Items { get;  } = [];

        public CreateOrderCommand(Guid userId, IReadOnlyCollection<CreateOrderItemDto> items)
        {
            UserId = userId;
            Items = items;
        }
    }
}
