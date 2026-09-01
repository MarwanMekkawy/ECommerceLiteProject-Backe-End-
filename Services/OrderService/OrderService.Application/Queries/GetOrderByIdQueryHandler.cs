using AutoMapper;
using Domain.Exceptions;
using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;
using OrderService.Domain.Contracts;

namespace OrderService.Application.Queries
{
    public class GetOrderByIdQueryHandler(IOrderRepository orderRepository, IMapper mapper) : IQueryHandler<GetOrderByIdQuery, OrderResponseDto?>
    {
        public async Task<OrderResponseDto?> HandleAsync(GetOrderByIdQuery query, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdAndUserIdUnTrackedAsync(query.OrderId, query.UserId, cancellationToken);

            if (order is null)
                throw new NotFoundException($"Order with Id [{query.OrderId}] was NOT FOUND.");

            return mapper.Map<OrderResponseDto>(order);
        }
    }
}
