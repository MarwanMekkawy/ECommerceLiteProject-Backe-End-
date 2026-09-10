using NotificationService.Application.Abstractions;
using NotificationService.Domain.Contracts;
using System.Text.Json;

namespace NotificationService.Application.Services
{
    public class NotificationDeliveryService(IMailjetClient mailClient, IUnitOfWork uow) : INotificationDeliveryService
    {
        public async Task ProcessPendingNotificationsAsync(CancellationToken cancellationToken)
        {
            var notifications = await uow.NotificationRepo.GetPendingAsync(cancellationToken);

            foreach (var notification in notifications)
            {
                notification.AttemptToSend();

                try
                {
                    var variables = notification.Data is null? null : JsonSerializer.Deserialize<Dictionary<string, object>>(notification.Data);

                    await mailClient.SendTemplateAsync(notification.RecipientEmail.Value, null, notification.Type, notification.Id, variables);

                    notification.MarkAsSent();
                }
                catch (Exception ex)
                {
                    notification.MarkAsFailed(ex.Message);
                }
            }
            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
