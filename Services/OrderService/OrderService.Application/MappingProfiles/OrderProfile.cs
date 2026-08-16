using AutoMapper;
using OrderService.Application.DTOs;
using OrderService.Domain.Orders;

namespace OrderService.Application.MappingProfiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderResponseDto>();
            CreateMap<OrderItem, OrderItemResponseDto>();
        }
    }
}
