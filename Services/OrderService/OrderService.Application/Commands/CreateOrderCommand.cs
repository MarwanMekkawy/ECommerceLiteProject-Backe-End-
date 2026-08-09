using OrderService.Application.DTOs;

namespace OrderService.Application.Commands
{
    public class CreateOrderCommand
    {
        public Guid UserId { get; set; }

        public IReadOnlyCollection<CreateOrderItemDto> Items { get; set; } = [];
    }
}
