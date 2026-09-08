namespace NotificationService.Application.Abstractions
{
    public interface IMailjetClient
    {
        Task SendTemplateAsync(string recipientEmail, string? recipientName, int templateId, Guid notificationId, IReadOnlyDictionary<string, object>? variables);
    }
}
