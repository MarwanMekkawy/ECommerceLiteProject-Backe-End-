using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Abstractions;
using NotificationService.Domain.Contracts;
using NotificationService.Infrastructure.Clients.MailjetClient;
using NotificationService.Infrastructure.Repositories;

namespace NotificationService.Infrastructure.Extentions.Infra
{
    public static class InfrastructureServicesCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            //DbContext Connection String 
            services.AddAppDbContext(config);

            // DI registering
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IProcessedMailjetEventRepository, IProcessedMailjetEventRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMailjetClient, MailjetClient>();

            return services;
        }
    }
}
