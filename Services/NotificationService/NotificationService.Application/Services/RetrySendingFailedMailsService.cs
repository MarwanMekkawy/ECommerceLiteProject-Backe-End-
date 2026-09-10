using NotificationService.Application.Abstractions;
using NotificationService.Domain.Contracts;
using System.Text.Json;

namespace NotificationService.Application.Services
{
    public class RetrySendingFailedMailsService(IMailjetClient mailClient, IUnitOfWork uow) : IRetrySendingFailedMailsService
    {
        public async Task RetryFailedNotificationsAsync(CancellationToken cancellationToken)
        {
            var notifications = await uow.NotificationRepo.GetFailedNotificationsDueForRetryAsync(cancellationToken);

            foreach (var notification in notifications)
            {
                notification.AttemptToSend();

                try
                {
                    var variables = notification.Data is null ? null : JsonSerializer.Deserialize<Dictionary<string, object>>(notification.Data);

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
