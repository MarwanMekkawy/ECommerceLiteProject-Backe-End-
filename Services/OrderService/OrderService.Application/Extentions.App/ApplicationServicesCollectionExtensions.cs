using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Commands;
using OrderService.Application.Queries;

namespace OrderService.Application.Extentions.App
{
    public static class ApplicationServicesCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Commands
            services.AddScoped<CreateOrderCommandHandler>();
            services.AddScoped<ConfirmOrderCommandHandler>();
            services.AddScoped<CompleteOrderCommandHandler>();
            services.AddScoped<CancelOrderCommandHandler>();

            // Queries
            services.AddScoped<GetOrderByIdQueryHandler>();
            services.AddScoped<GetOrdersByUserQueryHandler>();
            services.AddScoped<GetAllOrdersQueryHandler>();
            services.AddScoped<GetOrderByIdAdminQueryHandler>();
          
            return services;
        }
    }
}
