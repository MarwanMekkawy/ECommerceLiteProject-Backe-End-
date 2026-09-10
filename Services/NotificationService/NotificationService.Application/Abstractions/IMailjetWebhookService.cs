using NotificationService.Application.DTOs;

namespace NotificationService.Application.Abstractions
{
    public interface IMailjetWebhookService
    {
        Task ProcessEventAsync(MailjetWebhookRequest request, CancellationToken cancellationToken);
    }
}
