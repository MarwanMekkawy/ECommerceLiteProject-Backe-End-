using Domain.Exceptions;
using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;
using OrderService.Domain.Contracts;
using OrderService.Domain.Enums;
using OrderService.Domain.Exceptions.DomainExceptions;
using OrderService.Domain.Orders;

namespace OrderService.Application.Commands
{
    public class CheckoutOrderCommandHandler(IOrderRepository orderRepository, IProductServiceClient productServiceClient)
        : ICommandHandler<CheckoutOrderCommand, CheckoutOrderDto>
    {
        public Task<CheckoutOrderDto> HandleAsync(CheckoutOrderCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
