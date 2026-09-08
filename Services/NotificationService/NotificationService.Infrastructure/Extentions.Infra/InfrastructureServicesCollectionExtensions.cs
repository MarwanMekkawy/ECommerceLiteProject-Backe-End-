using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Abstractions;
using NotificationService.Infrastructure.Clients.MailjetClient;

namespace NotificationService.Infrastructure.Extentions.Infra
{
    public static class InfrastructureServicesCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped
            services.AddScoped<IMailjetClient, MailjetClient>();

            return services;
        }
    }
}
