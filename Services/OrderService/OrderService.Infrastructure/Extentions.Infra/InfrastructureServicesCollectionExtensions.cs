using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Abstractions.ClientsAbstractions;
using OrderService.Domain.Contracts;
using OrderService.Infrastructure.Clients.IdentityServiceTokenAuthClient;
using OrderService.Infrastructure.Clients.NotificationServiceClient;
using OrderService.Infrastructure.Clients.PaymentServiceClient;
using OrderService.Infrastructure.Clients.ProductServiceClient;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Infrastructure.Extentions.Infra
{
    public static class InfrastructureServicesCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            //DbContext Connection String 
            services.AddAppDbContext(config);

            // DI registering
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Clients
            
            services.AddHttpClient<IPaymentServiceClient, PaymentServiceClient>(
                client => { client.BaseAddress = new Uri(config["HttpClients:PaymentService:BaseUrl"]!); });
            services.AddHttpClient<IProductServiceClient, ProductServiceClient>(
                client =>{client.BaseAddress = new Uri(config["HttpClients:ProductService:BaseUrl"]!);});
            services.AddHttpClient<IServiceTokenClient, ServiceTokenClient>(
                client =>{client.BaseAddress = new Uri(config["HttpClients:IdentityService:BaseUrl"]!);});
            services.AddHttpClient<INotificationServiceClient, NotificationServiceClient>(
                client => { client.BaseAddress = new Uri(config["HttpClients:NotificationService:BaseUrl"]!);});
            services.AddSingleton<IServiceTokenCache, ServiceTokenCache>();

            return services;
        }
    }
}
