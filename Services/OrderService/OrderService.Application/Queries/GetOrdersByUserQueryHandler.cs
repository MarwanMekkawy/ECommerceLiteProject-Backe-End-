using AutoMapper;
using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;
using OrderService.Domain.Contracts;

namespace OrderService.Application.Queries
{
    public class GetOrdersByUserQueryHandler(IOrderRepository orderRepository, IMapper mapper) : IQueryHandler<GetOrdersByUserQuery, IReadOnlyList<OrderResponseDto>>
    {
        public async Task<IReadOnlyList<OrderResponseDto>> HandleAsync(GetOrdersByUserQuery query, CancellationToken cancellationToken)
        {
            var orders = await orderRepository.GetPagedByUserIdAsync(query.UserId, query.PageNumber, query.PageSize, cancellationToken);

            return mapper.Map<IReadOnlyList<OrderResponseDto>>(orders);
        }
    }
}
