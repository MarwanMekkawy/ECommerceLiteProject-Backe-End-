using NotificationService.Domain.Enums;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Application.DTOs
{
    public class SendEmailNotificationRequestDto
    {
        public Guid UserId { get; set; }
        public string RecipientEmail { get; set; } = null!;
        public NotificationType Type { get; set; }
        public Dictionary<string, object> Data { get; set; } = [];
    }
}
