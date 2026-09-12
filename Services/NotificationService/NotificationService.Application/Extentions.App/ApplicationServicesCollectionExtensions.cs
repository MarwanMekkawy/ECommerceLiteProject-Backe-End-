using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Abstractions;
using NotificationService.Application.Services;

namespace NotificationService.Application.Extentions.App
{
    public static class ApplicationServicesCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<INotificationCreationService, NotificationCreationService>();
            services.AddScoped<INotificationDeliveryService, NotificationDeliveryService>();
            services.AddScoped<IRetrySendingFailedMailsService, RetrySendingFailedMailsService>();
            services.AddScoped<IMailjetWebhookService, MailjetWebhookService>();

            return services;
        }
    }
}
