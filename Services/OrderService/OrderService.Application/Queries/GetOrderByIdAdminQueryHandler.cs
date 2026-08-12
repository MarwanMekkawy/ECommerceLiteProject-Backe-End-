using AutoMapper;
using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;
using OrderService.Domain.Contracts;

namespace OrderService.Application.Queries
{
    public class GetOrderByIdAdminQueryHandler(IOrderRepository orderRepository, IMapper mapper) : IQueryHandler<GetOrderByIdAdminQuery, OrderResponseDto?>
    {
        public async Task<OrderResponseDto?> HandleAsync(GetOrderByIdAdminQuery query, CancellationToken cancellationToken = default)
        {
            var order = await orderRepository.GetByIdUntrackedAsync(query.OrderId, cancellationToken);

            return mapper.Map<OrderResponseDto>(order);
        }
    }
}
