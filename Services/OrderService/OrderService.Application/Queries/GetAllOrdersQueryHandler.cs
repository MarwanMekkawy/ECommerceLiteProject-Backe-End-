using AutoMapper;
using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;
using OrderService.Domain.Contracts;

namespace OrderService.Application.Queries
{
    public class GetAllOrdersQueryHandler(IOrderRepository orderRepository, IMapper mapper) : IQueryHandler<GetAllOrdersQuery, IReadOnlyList<OrderResponseDto>>
    {
        public async Task<IReadOnlyList<OrderResponseDto>> HandleAsync(GetAllOrdersQuery query, CancellationToken cancellationToken)
        {
            var orders = await orderRepository.GetPagedAsync(query.PageNumber, query.PageSize, cancellationToken);

            return mapper.Map<IReadOnlyList<OrderResponseDto>>(orders);
        }
    }
}
