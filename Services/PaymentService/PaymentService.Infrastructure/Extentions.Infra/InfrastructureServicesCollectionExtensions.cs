using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentService.Application.Abstractions;
using PaymentService.Domain.Contracts;
using PaymentService.Infrastructure.Clients;
using PaymentService.Infrastructure.Clients.ServiceTokenAuth;
using PaymentService.Infrastructure.Clients.Stripe;
using PaymentService.Infrastructure.Repositories;
using Stripe;

namespace PaymentService.Infrastructure.Extentions.Infra
{
    public static class InfrastructureServicesCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            //DbContext Connection String 
            services.AddAppDbContext(config);

            //stripe global configrations
            StripeConfiguration.ApiKey = config["Stripe:SecretKey"] ?? throw new InvalidOperationException("Stripe secret key is not configured.");

            // DI registering         
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IProcessedStripeEventRepository, ProcessedStripeEventRepository>();

            services.AddScoped<RefundService>();
            services.AddScoped<PaymentIntentService>();
            services.AddScoped<IStripePaymentClient, StripePaymentClient>();

            // Clients
            services.AddHttpClient<IOrderServiceClient, OrderServiceClient>(
                client =>{client.BaseAddress = new Uri(config["HttpClients:OrderService:BaseUrl"]!); });
            services.AddHttpClient<IServiceTokenClient, ServiceTokenClient>(
                client => { client.BaseAddress = new Uri(config["HttpClients:IdentityService:BaseUrl"]!); });
            services.AddSingleton<IServiceTokenCache, ServiceTokenCache>();

            return services;
        }
    }
}