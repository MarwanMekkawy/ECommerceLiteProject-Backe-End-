using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;

namespace OrderService.Application.Commands
{
    public class CreateOrderCommand : ICommand
    {
        public Guid UserId { get; }
        public string Email { get; }

        public IReadOnlyCollection<CreateOrderItemDto> Items { get;  } = [];

        public CreateOrderCommand(Guid userId,string email, IReadOnlyCollection<CreateOrderItemDto> items)
        {
            UserId = userId;
            Email = email;
            Items = items;
        }
    }
}
