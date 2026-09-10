using NotificationService.Domain.Enums;

namespace NotificationService.Application.Abstractions
{
    public interface IMailjetClient
    {
        Task SendTemplateAsync(string recipientEmail, string? recipientName, NotificationType Type, Guid notificationId, IReadOnlyDictionary<string, object>? variables);
    }
}
