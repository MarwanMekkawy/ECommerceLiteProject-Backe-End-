using OrderService.Application.Abstractions;

namespace OrderService.Application.Commands
{
    public class AddOrderItemCommand : ICommand
    {
        public Guid UserId { get; }
        public string Email { get; }
        public Guid ProductId { get; }
        public int Quantity { get; }

        public AddOrderItemCommand(Guid userId,string email, Guid productId, int quantity)
        {
            UserId = userId;
            Email = email;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
