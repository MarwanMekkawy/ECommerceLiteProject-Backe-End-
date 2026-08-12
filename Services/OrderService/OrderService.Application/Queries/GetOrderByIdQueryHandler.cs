using AutoMapper;
using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;
using OrderService.Domain.Contracts;

namespace OrderService.Application.Queries
{
    public class GetOrderByIdQueryHandler(IOrderRepository orderRepository, IMapper mapper) : IQueryHandler<GetOrderByIdQuery, OrderResponseDto?>
    {
        public async Task<OrderResponseDto?> HandleAsync(GetOrderByIdQuery query, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdAndUserIdUnTrackedAsync(query.UserId, query.OrderId, cancellationToken);

            return mapper.Map<OrderResponseDto>(order);
        }
    }
}
