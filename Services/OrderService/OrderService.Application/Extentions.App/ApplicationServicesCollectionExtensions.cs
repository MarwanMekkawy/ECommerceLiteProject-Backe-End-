using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Abstractions;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Application.MappingProfiles;
using OrderService.Application.Queries;
using OrderService.Domain.Orders;

namespace OrderService.Application.Extentions.App
{
    public static class ApplicationServicesCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Commands
            services.AddScoped<ICommandHandler<CreateOrderCommand>, CreateOrderCommandHandler>();
            services.AddScoped<ICommandHandler<CheckoutOrderCommand, CheckoutOrderDto>, CheckoutOrderCommandHandler>();
            services.AddScoped<ICommandHandler<CompleteOrderInternalCommand>, CompleteOrderCommandInternalHandler>();
            services.AddScoped<ICommandHandler<CancelOrderCommand>, CancelOrderCommandHandler>();
            services.AddScoped<ICommandHandler<CancelOrderInternalCommand>, CancelOrderInternalCommandHandler>();
            services.AddScoped<ICommandHandler<AddOrderItemCommand>, AddOrderItemCommandHandler>();
            services.AddScoped<ICommandHandler<DecreaseOrderItemCommand>, DecreaseOrderItemCommandHandler>();
            services.AddScoped<ICommandHandler<IncreaseOrderItemCommand>, IncreaseOrderItemCommandHandler>();


            // Queries
            services.AddScoped<IQueryHandler<GetOrderByIdQuery, Order?>, GetOrderByIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetOrdersByUserQuery, IReadOnlyList<Order>>, GetOrdersByUserQueryHandler>();
            services.AddScoped<IQueryHandler<GetAllOrdersQuery, IReadOnlyList<Order>>, GetAllOrdersQueryHandler>();
            services.AddScoped<IQueryHandler<GetLatestOrderQuery, Order?>, GetLatestOrderQueryHandler>();
            services.AddScoped<IQueryHandler<GetOrderByIdAdminQuery, Order?>, GetOrderByIdAdminQueryHandler>();

            services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(AutoMapperMarker).Assembly); });

            return services;
        }
    }
}
