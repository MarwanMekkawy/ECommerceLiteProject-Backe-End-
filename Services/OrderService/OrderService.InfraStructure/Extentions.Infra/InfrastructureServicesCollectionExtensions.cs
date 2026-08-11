using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Abstractions;
using OrderService.Domain.Contracts;
using OrderService.InfraStructure.Clients;
using OrderService.InfraStructure.Repositories;

namespace OrderService.InfraStructure.Extentions.Infra
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

            // Product Clients
            services.AddHttpClient<IProductServiceClient, ProductServiceClient>(
                client =>{client.BaseAddress = new Uri(config["HttpClients:ProductService:BaseUrl"]!);});
            services.AddHttpClient<IServiceTokenClient, ServiceTokenClient>(
                client =>{client.BaseAddress = new Uri(config["HttpClients:IdentityService:BaseUrl"]!);});
            services.AddSingleton<IServiceTokenCache, ServiceTokenCache>();

            return services;
        }
    }
}
