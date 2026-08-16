using AutoMapper;
using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;
using OrderService.Domain.Contracts;


namespace OrderService.Application.Queries
{
    public class GetLatestOrderQueryHandler(IOrderRepository orderRepository, IMapper mapper) : IQueryHandler<GetLatestOrderQuery, OrderResponseDto?>
    {
        public async Task<OrderResponseDto?> HandleAsync(GetLatestOrderQuery query, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetLatestByUserIdUnTrackedAsync(query.UserId, cancellationToken);

            return mapper.Map<OrderResponseDto>(order);
        }
    }
}
