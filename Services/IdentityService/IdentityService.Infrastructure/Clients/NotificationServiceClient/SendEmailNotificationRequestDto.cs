using System.Text.Json.Serialization;

namespace IdentityService.Infrastructure.Clients.NotificationServiceClient
{
    public class SendEmailNotificationRequestDto
    {
        public Guid UserId { get; set; }
        public string RecipientEmail { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NotificationType Type { get; set; }

        public Dictionary<string, object> Data { get; set; } = [];
    }
}
